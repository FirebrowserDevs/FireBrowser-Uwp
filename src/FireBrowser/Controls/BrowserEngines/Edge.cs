#nullable enable
using Microsoft.Web.WebView2.Core;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Win32;
using Windows.Win32.Foundation;
using SysPoint = System.Drawing.Point;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.UI.Input;
using Windows.Devices.Input;
using System.Diagnostics;
using Windows.Globalization;
using Windows.UI.ViewManagement;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.System.SystemServices;
using Windows.Win32.UI.Input.Pointer;
using Size = Windows.Foundation.Size;
using Windows.UI.Xaml.Documents;
using Point = Windows.Foundation.Point;
using Windows.System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using Windows.Graphics.Display;
using Windows.System.Threading;
using Windows.UI.Composition.Interactions;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace FireBrowser.Controls.BrowserEngines
{
    [ComImport, Guid("45D64A29-A63E-4CB6-B498-5781D298CB4F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ICoreWindowInterop
    {
        System.IntPtr WindowHandle { get; }
        bool MessageHandled { set; }
    }
    [Flags]
    public enum MOUSEHOOKSTRUCTEX_MOUSE_DATA : uint
    {
        XBUTTON1 = 0x10000,
        XBUTTON2 = 0x20000,
    }
    class ElementInteractionTracker : IInteractionTrackerOwner
    {
        public InteractionTracker InteractionTracker { get; }
        public VisualInteractionSource ScrollPresenterVisualInteractionSource { get; }
        public ElementInteractionTracker(UIElement element)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);
            InteractionTracker = InteractionTracker.CreateWithOwner(visual.Compositor, this);
            InteractionTracker.MinPosition = new System.Numerics.Vector3(-1000, -1000, -1000);
            InteractionTracker.MaxPosition = new System.Numerics.Vector3(1000, 1000, 1000);
            InteractionTracker.MinScale = 0.5f;
            InteractionTracker.MaxScale = 5f;

            InteractionTracker.InteractionSources.Add(
                ScrollPresenterVisualInteractionSource = VisualInteractionSource.Create(visual)
            );
            ScrollPresenterVisualInteractionSource.IsPositionXRailsEnabled =
                ScrollPresenterVisualInteractionSource.IsPositionYRailsEnabled = true;


            ScrollPresenterVisualInteractionSource.PointerWheelConfig.PositionXSourceMode =
                ScrollPresenterVisualInteractionSource.PointerWheelConfig.PositionYSourceMode
                = InteractionSourceRedirectionMode.Enabled;

            ScrollPresenterVisualInteractionSource.PositionXChainingMode =
                ScrollPresenterVisualInteractionSource.ScaleChainingMode =
                InteractionChainingMode.Auto;

            ScrollPresenterVisualInteractionSource.PositionXSourceMode =
                ScrollPresenterVisualInteractionSource.PositionYSourceMode =
                ScrollPresenterVisualInteractionSource.ScaleSourceMode =
                InteractionSourceMode.EnabledWithInertia;

        }
        public void CustomAnimationStateEntered(InteractionTracker sender, InteractionTrackerCustomAnimationStateEnteredArgs args)
        {

        }

        public void IdleStateEntered(InteractionTracker sender, InteractionTrackerIdleStateEnteredArgs args)
        {

        }

        public void InertiaStateEntered(InteractionTracker sender, InteractionTrackerInertiaStateEnteredArgs args)
        {

        }

        public void InteractingStateEntered(InteractionTracker sender, InteractionTrackerInteractingStateEnteredArgs args)
        {

        }

        public void RequestIgnored(InteractionTracker sender, InteractionTrackerRequestIgnoredArgs args)
        {

        }
        Vector3 Vec = new Vector3(1000, 1000, 1000);
        public void ValuesChanged(InteractionTracker sender, InteractionTrackerValuesChangedArgs args)
        {
            InteractionTracker.MinPosition = args.Position - Vec;
            InteractionTracker.MaxPosition = args.Position + Vec;
            ValuesChangedEvent?.Invoke(args);
        }
        public event Action<InteractionTrackerValuesChangedArgs>? ValuesChangedEvent;
    }
    public class Edge : UserControl, IBrowserEngine
    {
        delegate bool ClientToScreenDelegate(HWND hWnd, ref SysPoint lpPoint);
        delegate int SendMessageDelegate(HWND hWnd, uint Msg, WPARAM wParam, LPARAM lParam);
        delegate IntPtr CreateWindowExDelegate(
            uint dwExStyle,
            string lpClassName,
            string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam
        );
        delegate IntPtr DefWindowProcDelegate(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
        delegate HWND GetFocusDelegate();
        delegate ushort RegisterClassDelegate(in WNDCLASSW lpWndClass);
        delegate BOOL DestroyWindowDelegate(HWND hWnd);

        static readonly ClientToScreenDelegate ClientToScreen;
        static readonly SendMessageDelegate SendMessage;
        static readonly CreateWindowExDelegate CreateWindowEx;
        static readonly WNDPROC DefWindowProc;
        static readonly GetFocusDelegate GetFocus;
        static readonly RegisterClassDelegate RegisterClass;
        static readonly DestroyWindowDelegate DestroyWindow;
        //Implementing IBrowserEngine
        public bool CanGoBack { get { return m_coreWebView.CanGoBack; } }
        public bool CanGoForward { get { return m_coreWebView.CanGoForward; } }

        static Edge()
        {
            
            var user32module = PInvoke.GetModuleHandle("user32.dll");
            if (user32module == default)
            {
                user32module = PInvoke.GetModuleHandle("ext-ms-win-rtcore-webview-l1-1-0.dll");
            }
            if (user32module == default) Environment.FailFast("Failed to obtain user32 apis");
            var address = PInvoke.GetProcAddress(user32module, "ClientToScreen");
            ClientToScreen =
                Marshal.GetDelegateForFunctionPointer<ClientToScreenDelegate>(
                    address
                );
            SendMessage =
                Marshal.GetDelegateForFunctionPointer<SendMessageDelegate>(
                    PInvoke.GetProcAddress(user32module, "SendMessageW")
                );
            CreateWindowEx =
                Marshal.GetDelegateForFunctionPointer<CreateWindowExDelegate>(
                PInvoke.GetProcAddress(user32module, "CreateWindowExW")
                );
            DefWindowProc =
                Marshal.GetDelegateForFunctionPointer<WNDPROC>(
                    PInvoke.GetProcAddress(user32module, "DefWindowProcW")
                );
            GetFocus =
                Marshal.GetDelegateForFunctionPointer<GetFocusDelegate>(
                PInvoke.GetProcAddress(user32module, "GetFocus")
            );
            RegisterClass =
                Marshal.GetDelegateForFunctionPointer<RegisterClassDelegate>(
                    PInvoke.GetProcAddress(user32module, "RegisterClassW")
                );
            DestroyWindow = Marshal.GetDelegateForFunctionPointer<DestroyWindowDelegate>(
                PInvoke.GetProcAddress(user32module, "DestroyWindow")
            );
        }
        bool m_isClosed;
        long m_manipulationModeChangedToken, m_visibilityChangedToken;
        HWND m_tempHostHwnd, m_inputWindowHwnd;
        CoreWebView2? m_coreWebView;
        CoreWebView2EnvironmentOptions? m_options;
        CoreWebView2Environment? m_coreWebViewEnvironment;
        public CoreWebView2Controller? m_coreWebViewController;
        CoreWebView2CompositionController? m_coreWebViewCompositionController;
        public CoreWebView2CompositionController? CoreWebView2CompositionController => m_coreWebViewCompositionController;
        public CoreWebView2? CoreWebView2 => m_coreWebView;
        event Action? CoreWebView2Initialized;
        readonly ElementInteractionTracker ElementInteractionTracker;
        public Edge()
        {
       
            // MODIFIED
            ElementInteractionTracker = new(this);
            ElementInteractionTracker.ValuesChangedEvent += ElementInteractionTracker_ValuesChanged;
            PrevPosition = ElementInteractionTracker.ScrollPresenterVisualInteractionSource.Position;
            // END MODIFIED
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
          
            ManipulationStarted += Edge_ManipulationStarted;

            // Turn off DirectManipulation for this element so that all scrolling and gesture
            // inside the WebView element will be handled by Anaheim.
            ManipulationMode = ManipulationModes.None;

            // TODO_WebView2: These can be deferred to CreateCoreObjects
            m_manipulationModeChangedToken = RegisterPropertyChangedCallback(ManipulationModeProperty,
               OnManipulationModePropertyChanged);
            m_visibilityChangedToken = RegisterPropertyChangedCallback(VisibilityProperty,
               OnVisibilityPropertyChanged);
            OnVisibilityPropertyChanged(null, null);

            // IsTabStop is false by default for UIElement
            IsTabStop = true;

            // Set the background for WebView2 to ensure it will be visible to hit-testing.
            Background = new SolidColorBrush(Colors.Transparent);

           
        }

        private void Edge_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            //Debug.WriteLine("manipulation" + e.Position);
        }

        // MODIFIED
        Vector3 PrevPosition;
        async void ElementInteractionTracker_ValuesChanged(InteractionTrackerValuesChangedArgs obj)
        {
            if (CoreWebView2 is null) return;
            var delta = obj.Position - PrevPosition;
            PrevPosition = obj.Position;
            await CoreWebView2.CallDevToolsProtocolMethodAsync("Input.dispatchMouseEvent", @$"
{{
    ""type"": ""mouseWheel"",
    ""x"": {100},
    ""y"": {100},
    ""deltaX"": {delta.X},
    ""deltaY"": {delta.Y}
}}");
            await CoreWebView2.CallDevToolsProtocolMethodAsync("Emulation.setPageScaleFactor", @$"
{{
    ""pageScaleFactor"": {obj.Scale}
}}");
        }
        // END MODIFIED
        void OnManipulationModePropertyChanged(DependencyObject? sender, DependencyProperty? dp)
            => Environment.FailFast("WebView2.ManipulationMode cannot be set to anything other than \"None\".");

        void OnVisibilityPropertyChanged(DependencyObject? sender, DependencyProperty? dp)
        {
            UpdateRenderedSubscriptionAndVisibility();
        }
        public void Close()
        {
            CloseInternal(false);
        }
        ~Edge()
        {
            CloseInternal(true);
        }
        // Close Implementation (notice this does not implement IClosable). Also called implicitly as part of destruction.
        void CloseInternal(bool inShutdownPath)
        {
            DisconnectFromRootVisualTarget();

            //UnregisterCoreEventHandlers();

            var xamlRoot = XamlRoot;
            if (xamlRoot != null)
            {
                xamlRoot.Changed -= XamlRootChangedHanlder;
            }
            Window.Current.VisibilityChanged -= VisiblityChangedHandler;
            Windows.UI.Xaml.Media.CompositionTarget.Rendered -= HandleRendered;
            m_renderedRegistered = true;

            if (m_tempHostHwnd != default && CoreWindow.GetForCurrentThread() is null)
            {
                DestroyWindow(m_tempHostHwnd);
                m_tempHostHwnd = default;
            }

            if (m_manipulationModeChangedToken != 0)
            {
                UnregisterPropertyChangedCallback(ManipulationModeProperty, m_manipulationModeChangedToken);
                m_manipulationModeChangedToken = 0;
            }

            if (m_visibilityChangedToken != 0)
            {
                UnregisterPropertyChangedCallback(VisibilityProperty, m_visibilityChangedToken);
                m_visibilityChangedToken = 0;
            }

            m_inputWindowHwnd = default;

            if (m_coreWebView is not null)
                m_coreWebView = null;

            if (m_coreWebViewController is not null)
            {
                m_coreWebViewController.Close();
                m_coreWebViewController = null;
            }

            if (m_coreWebViewCompositionController is not null)
            {
                m_coreWebViewCompositionController = null;
            }

            UnregisterXamlEventHandlers();

            // If called from destructor, skip ResetProperties() as property values no longer matter.
            // (Otherwise, Xaml Core will assert on failure to resurrect WV2's DXaml Peer)
            if (!inShutdownPath)
            {
                ResetProperties();
            }

            m_isClosed = true;
        }
        bool
            m_hasMouseCapture,
            m_isLeftMouseButtonPressed,
            m_isMiddleMouseButtonPressed,
            m_isRightMouseButtonPressed,
            m_isXButton1Pressed,
            m_isXButton2Pressed,
            m_hasPenCapture,
            m_isPointerOver;
        readonly Dictionary<uint, bool> m_hasTouchCapture = new();
        void HandlePointerPressed(object sender, PointerRoutedEventArgs args)
        {
            // Chromium handles WM_MOUSEXXX for mouse, WM_POINTERXXX for touch
            PointerDeviceType deviceType = args.Pointer.PointerDeviceType;
            PointerPoint pointerPoint = args.GetCurrentPoint(this);
            uint message = 0x0;

            if (deviceType == PointerDeviceType.Mouse)
            {
                // WebView takes mouse capture to avoid missing pointer released events that occur outside of the element that
                // end pointer pressed state inside the webview. Example, scrollbar is being used and mouse is moved out
                // of webview bounds before being released, the webview will miss the released event and upon reentry into
                // the webview, the mouse will still cause the scrollbar to move as if selected.
                m_hasMouseCapture = CapturePointer(args.Pointer);

                PointerPointProperties properties = pointerPoint.Properties;
                if (properties.IsLeftButtonPressed)
                {
                    // Double Click is working as well with this code, presumably by being recognized on browser side from WM_LBUTTONDOWN/ WM_LBUTTONUP
                    message = PInvoke.WM_LBUTTONDOWN;
                    m_isLeftMouseButtonPressed = true;
                }
                else if (properties.IsMiddleButtonPressed)
                {
                    message = PInvoke.WM_MBUTTONDOWN;
                    m_isMiddleMouseButtonPressed = true;
                }
                else if (properties.IsRightButtonPressed)
                {
                    //to-do: temp test to make context menu work
                    message = PInvoke.WM_RBUTTONDOWN;
                    m_isRightMouseButtonPressed = true;
                }
                else if (properties.IsXButton1Pressed)
                {
                    message = PInvoke.WM_XBUTTONDOWN;
                    m_isXButton1Pressed = true;
                }
                else if (properties.IsXButton2Pressed)
                {
                    message = PInvoke.WM_XBUTTONDOWN;
                    m_isXButton2Pressed = true;
                }
                else
                {
                    Debugger.Break();
                    throw new InvalidOperationException("Should not reach here");
                }
            }
            else if (deviceType == PointerDeviceType.Touch)
            {
                message = PInvoke.WM_POINTERDOWN;

                uint id = pointerPoint.PointerId;
                m_hasTouchCapture.Add(id, CapturePointer(args.Pointer));
            }
            else if (deviceType == PointerDeviceType.Pen)
            {
                message = PInvoke.WM_POINTERDOWN;
                m_hasPenCapture = CapturePointer(args.Pointer);
            }

            // Since OnXamlPointerMessage() will mark the args handled, Xaml FocusManager will ignore
            // this Pressed event when it bubbles up to the XamlRoot, not setting focus as expected.
            // Thus, we need to manually set Xaml Focus (Pointer) on WebView2 here.
            // TODO_WebView2: Update this to UIElement.Focus [sync version], when it becomes available.

            if (IsTabStop)
            {
                _ = FocusManager.TryFocusAsync(this, FocusState.Pointer);
            }

            OnXamlPointerMessage(message, args);
        }

        void HandlePointerReleased(object sender, PointerRoutedEventArgs args)
        {
            // Chromium handles WM_MOUSEXXX for mouse, WM_POINTERXXX for touch
            PointerDeviceType deviceType = args.Pointer.PointerDeviceType;
            PointerPoint pointerPoint = args.GetCurrentPoint(this);
            uint message;

            // Get pointer id for handling multi-touch capture
            uint id = pointerPoint.PointerId;

            if (deviceType == PointerDeviceType.Mouse)
            {
                if (m_isLeftMouseButtonPressed)
                {
                    message = PInvoke.WM_LBUTTONUP;
                    m_isLeftMouseButtonPressed = false;
                }
                else if (m_isMiddleMouseButtonPressed)
                {
                    message = PInvoke.WM_MBUTTONUP;
                    m_isMiddleMouseButtonPressed = false;
                }
                else if (m_isRightMouseButtonPressed)
                {
                    message = PInvoke.WM_RBUTTONUP;
                    m_isRightMouseButtonPressed = false;
                }
                else if (m_isXButton1Pressed)
                {
                    message = PInvoke.WM_XBUTTONUP;
                    m_isXButton1Pressed = false;
                }
                else if (m_isXButton2Pressed)
                {
                    message = PInvoke.WM_XBUTTONUP;
                    m_isXButton2Pressed = false;
                }
                else
                {
                    // It is not guaranteed that we will get a PointerPressed before PointerReleased.
                    // For example, the mouse can be pressed in the space next to a webview, dragged
                    // into the webview, and then released. This is a valid case and should not crash.
                    // Because we can't always know what button was pressed before a release, we can't
                    // forward this message on to CoreWebView2.
                    return;
                }

                if (m_hasMouseCapture)
                {
                    ReleasePointerCapture(args.Pointer);
                    m_hasMouseCapture = false;
                }
            }
            else
            {
                if (m_hasTouchCapture.ContainsKey(id))
                {
                    ReleasePointerCapture(args.Pointer);
                    m_hasTouchCapture.Remove(id);
                }

                if (m_hasPenCapture)
                {
                    ReleasePointerCapture(args.Pointer);
                    m_hasPenCapture = false;
                }

                message = PInvoke.WM_POINTERUP;
            }

            OnXamlPointerMessage(message, args);
        }

        void HandlePointerMoved(object sender, PointerRoutedEventArgs args)
        {
            // Chromium handles WM_MOUSEXXX for mouse, WM_POINTERXXX for touch
            PointerDeviceType deviceType = args.Pointer.PointerDeviceType;
            uint message = deviceType == PointerDeviceType.Mouse ? PInvoke.WM_MOUSEMOVE : PInvoke.WM_POINTERUPDATE;
            OnXamlPointerMessage(message, args);
        }

        void HandlePointerWheelChanged(object sender, PointerRoutedEventArgs args)
        {
            // Chromium handles WM_MOUSEXXX for mouse, WM_POINTERXXX for touch
            PointerDeviceType deviceType = args.Pointer.PointerDeviceType;

            uint message = deviceType == PointerDeviceType.Mouse ?
                (args.GetCurrentPoint(this).Properties.IsHorizontalMouseWheel ? PInvoke.WM_MOUSEHWHEEL : PInvoke.WM_MOUSEWHEEL)
                : PInvoke.WM_POINTERWHEEL;
            OnXamlPointerMessage(message, args);
        }
        CoreCursor? m_oldCursor;
        void HandlePointerExited(object sender, PointerRoutedEventArgs args)
        {
            PointerDeviceType deviceType = args.Pointer.PointerDeviceType;
            uint message;

            if (m_isPointerOver)
            {
                m_isPointerOver = false;
                CoreWindow.GetForCurrentThread().PointerCursor = m_oldCursor;
                m_oldCursor = null;
            }

            if (deviceType == PointerDeviceType.Mouse)
            {
                message = PInvoke.WM_MOUSELEAVE;
                if (!m_hasMouseCapture)
                {
                    ResetMouseInputState();
                }
            }
            else
            {
                message = PInvoke.WM_POINTERLEAVE;
            }

            OnXamlPointerMessage(message, args);
        }

        void HandlePointerEntered(object sender, PointerRoutedEventArgs args)
        {
            PointerDeviceType deviceType = args.Pointer.PointerDeviceType;

            m_isPointerOver = true;
            m_oldCursor = CoreWindow.GetForCurrentThread().PointerCursor;

            UpdateCoreWindowCursor();

            if (deviceType != PointerDeviceType.Mouse) //mouse does not have an equivalent pointer_entered event, so only handling pen/touch
            {
                OnXamlPointerMessage(PInvoke.WM_POINTERENTER, args);
            }
        }

        void HandlePointerCanceled(object sender, PointerRoutedEventArgs args)
        {
            ResetPointerHelper(args);
        }

        void HandlePointerCaptureLost(object sender, PointerRoutedEventArgs args)
        {
            ResetPointerHelper(args);
        }

        void ResetPointerHelper(PointerRoutedEventArgs args)
        {
            PointerDeviceType deviceType = args.Pointer.PointerDeviceType;

            if (deviceType == PointerDeviceType.Mouse)
            {
                m_hasMouseCapture = false;
                ResetMouseInputState();
            }
            else if (deviceType == PointerDeviceType.Touch)
            {
                // Get pointer id for handling multi-touch capture
                PointerPoint logicalPointerPoint = args.GetCurrentPoint(this);
                uint id = logicalPointerPoint.PointerId;
                if (m_hasTouchCapture.ContainsKey(id))
                {
                    m_hasTouchCapture.Remove(id);
                }
            }
            else if (deviceType == PointerDeviceType.Pen)
            {
                m_hasPenCapture = false;
            }
        }
        HWND m_xamlHostHwnd;
        HWND GetHostHwnd()
        {
            if (m_xamlHostHwnd == default)
            {
                CoreWindow coreWindow = CoreWindow.GetForCurrentThread();
                if (coreWindow is not null)
                {
                    var coreWindowInterop = (ICoreWindowInterop)((dynamic)coreWindow);
                    m_xamlHostHwnd = new(coreWindowInterop.WindowHandle);
                }
            }

            return m_xamlHostHwnd;
        }
        HWND GetActiveInputWindowHwnd()
        {
            if (m_inputWindowHwnd == default)
            {
                var inputWindowHwnd = GetFocus();
                if (inputWindowHwnd == default)
                {
                    throw new COMException("A COM error has occured", Marshal.GetLastWin32Error());
                }
                Debug.Assert(inputWindowHwnd != m_xamlHostHwnd); // Focused XAML host window cannot be set as input hwnd
                m_inputWindowHwnd = inputWindowHwnd;
            }
            return m_inputWindowHwnd;
        }
        bool
            m_isImplicitCreationInProgress, m_isExplicitCreationInProgress,
            m_isCoreFailure_BrowserExited_State;


        readonly bool m_shouldShowMissingAnaheimWarning;
        async Task CreateCoreObjects()
        {
            Debug.Assert((m_isImplicitCreationInProgress && !m_isExplicitCreationInProgress) ||
                       (!m_isImplicitCreationInProgress && m_isExplicitCreationInProgress));

            if (m_isClosed)
            {
                throw new ObjectDisposedException("WebView2 is closed");
            }
            else
            {
                m_creationInProgressAsync = new TaskCompletionSource<bool>();
                RegisterXamlEventHandlers();

                // We are about to attempt re-creation of the environment,
                // so clear any previous 'Missing Anaheim Warning'
                //Content = null;

                if (!m_isCoreFailure_BrowserExited_State)
                {
                    // Normally we always need a new environment, the exception being when Anaheim process failed and we get to reuse the existing one
                    await CreateCoreEnvironment();
                }

                if (m_coreWebViewEnvironment != null)
                {
                    await CreateCoreWebViewFromEnvironment(GetHostHwnd());

                    // Do initialization including rendering setup.
                    // Try this now but defer to Loaded event if it hasn't fired yet.
                    TryCompleteInitialization();
                }
                else if (m_shouldShowMissingAnaheimWarning)
                {
                    CreateMissingAnaheimWarning();
                }
            }
        }
        async Task CreateCoreEnvironment()
        {
            string browserInstall = "";
            string userDataFolder = "";

            if (m_options is null)
            {
                // NOTE: To enable Anaheim logging, add: m_options.AdditionalBrowserArguments(L"--enable-logging=stderr --v=1");
                m_options = new CoreWebView2EnvironmentOptions();

                var applicationLanguagesList = ApplicationLanguages.Languages;
                if (applicationLanguagesList.Count > 0)
                {
                    m_options.Language = applicationLanguagesList[0];
                }
            }

            try
            {
                m_coreWebViewEnvironment = await CoreWebView2Environment.CreateWithOptionsAsync(
                    browserInstall,
                    userDataFolder,
                    m_options
                );
            }
            catch (COMException)
            {
                Debugger.Break();
                //hresult hr = e.code().value;
                //m_shouldShowMissingAnaheimWarning = hr == HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND);
                //FireCoreWebView2Initialized(hr);
            }
        }
        bool m_everHadCoreWebView;
        async Task CreateCoreWebViewFromEnvironment(HWND hwndParent)
        {
            if (hwndParent == default)
            {
                hwndParent = EnsureTemporaryHostHwnd();
            }

            try
            {
                var windowRef = CoreWebView2ControllerWindowReference.CreateFromWindowHandle((ulong)hwndParent.Value);
                // CreateCoreWebView2CompositionController(Async) creates a CompositionController and is in visual hosting mode.
                // Calling CreateCoreWebView2Controller would create a Controller and would be in windowed mode.
                m_coreWebViewCompositionController = await m_coreWebViewEnvironment!.CreateCoreWebView2CompositionControllerAsync(windowRef);
                m_coreWebViewController = m_coreWebViewCompositionController;
                m_coreWebViewController.ShouldDetectMonitorScaleChanges = false;
                m_coreWebView = m_coreWebViewController.CoreWebView2;
                m_everHadCoreWebView = true;
                m_coreWebViewCompositionController.CursorChanged += CoreWebView2CursorChanged;
                //RegisterCoreEventHandlers();
                // Creation is considered complete at this point, however rendering and accessibility
                // hookup will only run after we get Loaded event (see TryCompleteInitialization())
                CoreWebView2Initialized?.Invoke();
            }
            catch (COMException)
            {
                //FireCoreWebView2Initialized(e.ErrorCode);
            }

            m_isImplicitCreationInProgress = false;
            m_isExplicitCreationInProgress = false;
            m_creationInProgressAsync!.SetResult(true);
            m_creationInProgressAsync = new();
        }
        readonly static AccessibilitySettings AccessibilitySettings = new();
        void UpdateDefaultVisualBackgroundColor()
        {
            //var appResources = Application.Current.Resources;

            //var backgroundColorAsI =
            //    AccessibilitySettings.HighContrast ?
            //    appResources["SystemColorWindowColor"] :
            //    appResources["SolidBackgroundFillColorBase"];

            //Color backgroundColor = (Color)backgroundColorAsI;
        }
        unsafe HWND EnsureTemporaryHostHwnd()
        {
            // If we don't know the parent yet, either use the CoreWindow as the parent,
            // or if we don't have one, create a dummy hwnd to be the temporary parent.
            // Using a dummy parent all the time won't work, since we can't reparent the
            // browser from a Non-ShellManaged Hwnd (dummy) to a ShellManaged one (CoreWindow).
            CoreWindow coreWindow = CoreWindow.GetForCurrentThread();
            if (coreWindow is not null)
            {
                var coreWindowInterop = (ICoreWindowInterop)((dynamic)coreWindow);
                m_tempHostHwnd = new(coreWindowInterop.WindowHandle);
            }
            else
            {
                // Register the window class.
                string CLASS_NAME = "WEBVIEW2_TEMP_PARENT";
                HINSTANCE hInstance = PInvoke.GetModuleHandle(default(PCWSTR));
                fixed (char* classNameAsChars = CLASS_NAME)
                {
                    WNDCLASSW wc = new()
                    {
                        lpfnWndProc = DefWindowProc,
                        hInstance = hInstance,
                        lpszClassName = new(classNameAsChars)
                    };

                    RegisterClass(in wc);

                    m_tempHostHwnd = new(CreateWindowEx(
                        0,
                        CLASS_NAME,                                // Window class
                        "Webview2 Temporary Parent",               // Window text
                        (uint)WINDOW_STYLE.WS_OVERLAPPED,          // Window style
                        0, 0, 0, 0,
                        IntPtr.Zero,                               // Parent window
                        IntPtr.Zero,                               // Menu
                        hInstance,                                 // Instance handle
                        IntPtr.Zero                                // Additional application data
                    ));
                }
            }
            return m_tempHostHwnd;
        }

        void CreateMissingAnaheimWarning()
        {
            var warning = new TextBlock()
            {
                Inlines =
            {
                new Run { Text = "WebView2 Not Found, You can install it using this " },
                new Hyperlink { Inlines =
                    {
                        new Run {Text = "link" }
                    } , NavigateUri = new Uri("https://aka.ms/winui2/webview2download/") }
            }
            };
            Content = warning;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        // We could have a child Grid (see AddChildPanel) or a child TextBlock (see CreateMissingAnaheimWarning).
        // Make sure it is visited by the Arrange pass.
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Content is FrameworkElement child)
            {
                child.Arrange(new Rect(new Point(0, 0), finalSize));
                return finalSize;
            }

            return base.ArrangeOverride(finalSize);
        }

        void FillPointerPenInfo(PointerPoint inputPt, CoreWebView2PointerInfo outputPt)
        {
            PointerPointProperties inputProperties = inputPt.Properties;
            uint outputPt_penFlags = PInvoke.PEN_FLAG_NONE;

            if (inputProperties.IsBarrelButtonPressed)
            {
                outputPt_penFlags |= PInvoke.PEN_FLAG_BARREL;
            }

            if (inputProperties.IsInverted)
            {
                outputPt_penFlags |= PInvoke.PEN_FLAG_INVERTED;
            }

            if (inputProperties.IsEraser)
            {
                outputPt_penFlags |= PInvoke.PEN_FLAG_ERASER;
            }

            outputPt.PenFlags = outputPt_penFlags;

            uint outputPt_penMask = PInvoke.PEN_MASK_PRESSURE | PInvoke.PEN_MASK_ROTATION | PInvoke.PEN_MASK_TILT_X | PInvoke.PEN_MASK_TILT_Y;
            outputPt.PenMask = outputPt_penMask;

            uint outputPt_penPressure = (uint)inputProperties.Pressure * 1024;
            outputPt.PenPressure = outputPt_penPressure;

            uint outputPt_penRotation = (uint)inputProperties.Twist;
            outputPt.PenRotation = outputPt_penRotation;

            int outputPt_penTiltX = (int)inputProperties.XTilt;
            outputPt.PenTiltX = outputPt_penTiltX;

            int outputPt_penTiltY = (int)inputProperties.YTilt;
            outputPt.PenTiltY = outputPt_penTiltY;
        }
        double m_rasterizationScale = 1;
        void FillPointerTouchInfo(PointerPoint inputPt, CoreWebView2PointerInfo outputPt)
        {
            PointerPointProperties inputProperties = inputPt.Properties;

            outputPt.TouchFlags = PInvoke.TOUCH_FLAG_NONE;

            uint outputPt_touchMask = PInvoke.TOUCH_MASK_CONTACTAREA | PInvoke.TOUCH_MASK_ORIENTATION | PInvoke.TOUCH_MASK_ORIENTATION;
            outputPt.TouchMask = outputPt_touchMask;

            //TOUCH CONTACT
            double width = inputProperties.ContactRect.Width * m_rasterizationScale;
            double height = inputProperties.ContactRect.Height * m_rasterizationScale;
            double leftVal = inputProperties.ContactRect.X * m_rasterizationScale;
            double topVal = inputProperties.ContactRect.Y * m_rasterizationScale;

            Rect outputPt_touchContact = new(leftVal, topVal, width, height);
            outputPt.TouchContact = outputPt_touchContact;

            //TOUCH CONTACT RAW
            double widthRaw = inputProperties.ContactRectRaw.Width * m_rasterizationScale;
            double heightRaw = inputProperties.ContactRectRaw.Height * m_rasterizationScale;
            double leftValRaw = inputProperties.ContactRectRaw.X * m_rasterizationScale;
            double topValRaw = inputProperties.ContactRectRaw.Y * m_rasterizationScale;

            Rect outputPt_touchContactRaw = new(leftValRaw, topValRaw, widthRaw, heightRaw);
            outputPt.TouchContactRaw = outputPt_touchContactRaw;

            uint outputPt_touchOrientation = (uint)inputProperties.Orientation;
            outputPt.TouchOrientation = outputPt_touchOrientation;

            uint outputPt_touchPressure = (uint)(inputProperties.Pressure * 1024);
            outputPt.TouchPressure = outputPt_touchPressure;
        }
        unsafe void FillPointerInfo(PointerPoint inputPt, CoreWebView2PointerInfo outputPt, PointerRoutedEventArgs args)
        {
            PointerPointProperties inputProperties = inputPt.Properties;

            //DEVICE TYPE
            PointerDeviceType deviceType = inputPt.PointerDevice.PointerDeviceType;

            if (deviceType == PointerDeviceType.Pen)
            {
                outputPt.PointerKind = (uint)POINTER_INPUT_TYPE.PT_PEN;
            }
            else if (deviceType == PointerDeviceType.Touch)
            {
                outputPt.PointerKind = (uint)POINTER_INPUT_TYPE.PT_TOUCH;
            }

            outputPt.PointerId = args.Pointer.PointerId;

            outputPt.FrameId = inputPt.FrameId;

            //POINTER FLAGS
            POINTER_FLAGS outputPt_pointerFlags = POINTER_FLAGS.POINTER_FLAG_NONE;

            if (inputProperties.IsInRange)
            {
                outputPt_pointerFlags |= POINTER_FLAGS.POINTER_FLAG_INRANGE;
            }

            if (deviceType == PointerDeviceType.Touch)
            {
                if (inputPt.IsInContact)
                {
                    outputPt_pointerFlags |= POINTER_FLAGS.POINTER_FLAG_INCONTACT;
                    outputPt_pointerFlags |= POINTER_FLAGS.POINTER_FLAG_FIRSTBUTTON;
                }

                if (inputProperties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
                {
                    outputPt_pointerFlags |= POINTER_FLAGS.POINTER_FLAG_NEW;
                }
            }

            if (deviceType == PointerDeviceType.Pen)
            {
                if (inputPt.IsInContact)
                {
                    outputPt_pointerFlags |= POINTER_FLAGS.POINTER_FLAG_INCONTACT;

                    if (!inputProperties.IsBarrelButtonPressed)
                    {
                        outputPt_pointerFlags |= POINTER_FLAGS.POINTER_FLAG_FIRSTBUTTON;
                    }

                    else
                    {
                        outputPt_pointerFlags |= POINTER_FLAGS.POINTER_FLAG_SECONDBUTTON;
                    }
                } // POINTER_FLAG_NEW is currently omitted for pen input
            }

            if (inputProperties.IsPrimary)
            {
                outputPt_pointerFlags |= POINTER_FLAGS.POINTER_FLAG_PRIMARY;
            }

            if (inputProperties.TouchConfidence)
            {
                outputPt_pointerFlags |= POINTER_FLAGS.POINTER_FLAG_CONFIDENCE;
            }

            if (inputProperties.IsCanceled)
            {
                outputPt_pointerFlags |= POINTER_FLAGS.POINTER_FLAG_CANCELED;
            }

            if (inputProperties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
            {
                outputPt_pointerFlags |= POINTER_FLAGS.POINTER_FLAG_DOWN;
            }

            if (inputProperties.PointerUpdateKind == PointerUpdateKind.Other)
            {
                outputPt_pointerFlags |= POINTER_FLAGS.POINTER_FLAG_UPDATE;
            }

            if (inputProperties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
            {
                outputPt_pointerFlags |= POINTER_FLAGS.POINTER_FLAG_UP;
            }

            outputPt.PointerFlags = (uint)outputPt_pointerFlags;

            Point outputPt_pointerPixelLocation = new((m_rasterizationScale * inputPt.Position.X), (m_rasterizationScale * inputPt.Position.Y));
            outputPt.PixelLocation = outputPt_pointerPixelLocation;

            //HIMETRIC LOCATION (task 30544057 exists to finish this)
            //var himetricScale = 26.4583; //1 hiMetric = 0.037795280352161 PX
            //Point outputPt_pointerHimetricLocation(static_cast<float>(inputPt.Position().X), static_cast<float>(inputPt.Position().Y));
            //outputPt->HimetricLocation(outputPt_pointerHimetricLocation);

            Point outputPt_pointerRawPixelLocation = new(m_rasterizationScale * inputPt.RawPosition.X, m_rasterizationScale * inputPt.RawPosition.Y);
            outputPt.PixelLocationRaw = outputPt_pointerRawPixelLocation;

            //RAW HIMETRIC LOCATION
            //Point outputPt_pointerRawHimetricLocation = { static_cast<float>(inputPt.RawPosition().X), static_cast<float>(inputPt.RawPosition().Y) };
            //outputPt.HimetricLocationRaw(outputPt_pointerRawHimetricLocation);

            uint outputPoint_pointerTime = (uint)(inputPt.Timestamp / 1000); //microsecond to millisecond conversion(for tick count)
            outputPt.Time = outputPoint_pointerTime;

            var outputPoint_pointerHistoryCount = (uint)(args.GetIntermediatePoints(this).Count);
            outputPt.HistoryCount = outputPoint_pointerHistoryCount;

            //PERFORMANCE COUNT
            long lpFrequency = default;
            bool res = PInvoke.QueryPerformanceFrequency(&lpFrequency);
            if (res)
            {
                var scale = 1000000;
                var frequency = (ulong)lpFrequency;
                var outputPoint_pointerPerformanceCount = (inputPt.Timestamp * frequency) / (ulong)scale;
                outputPt.PerformanceCount = outputPoint_pointerPerformanceCount;
            }

            var outputPoint_pointerButtonChangeKind = (int)inputProperties.PointerUpdateKind;
            outputPt.ButtonChangeKind = outputPoint_pointerButtonChangeKind;
        }

        void UpdateCoreWindowCursor()
        {
            if (m_coreWebViewCompositionController is not null && m_isPointerOver)
            {
                try
                {
                    CoreWindow.GetForCurrentThread().PointerCursor = m_coreWebViewCompositionController.Cursor;
                }
                catch (Exception ex) {  }
            }
        }
        static class WebView2Utility
        {
            public static LPARAM PackIntoWin32StylePointerArgs_lparam(
            uint _,
            PointerRoutedEventArgs _1, Point point)
            {
                // These are the same for WM_POINTER and WM_MOUSE based events
                // Pointer: https://msdn.microsoft.com/en-us/ie/hh454929(v=vs.80)
                // Mouse: https://docs.microsoft.com/en-us/windows/desktop/inputdev/wm-mousemove
                LPARAM lParam = MAKELPARAM((int)point.X, (int)point.Y);
                return lParam;
            }

            public static WPARAM PackIntoWin32StyleMouseArgs_wparam(
            uint message,
            PointerRoutedEventArgs args, PointerPoint pointerPoint)
            {
                ushort lowWord = 0x0;       // unsigned modifier flags
                ushort highWord = 0x0;     // signed wheel delta

                VirtualKeyModifiers modifiers = args.KeyModifiers;

                // can support cases like Ctrl|Alt + Scroll where Alt will be ignored and it will be treated as Ctrl + Scroll
                if (((int)modifiers & (int)VirtualKeyModifiers.Control) != 0)
                {
                    lowWord |= (ushort)MODIFIERKEYS_FLAGS.MK_CONTROL;
                }
                if (((int)modifiers & (int)VirtualKeyModifiers.Shift) != 0)
                {
                    lowWord |= (ushort)MODIFIERKEYS_FLAGS.MK_SHIFT;
                }

                PointerPointProperties properties = pointerPoint.Properties;

                if (properties.IsLeftButtonPressed)
                {
                    lowWord |= (ushort)MODIFIERKEYS_FLAGS.MK_LBUTTON;
                }
                if (properties.IsRightButtonPressed)
                {
                    lowWord |= (ushort)MODIFIERKEYS_FLAGS.MK_RBUTTON;
                }
                if (properties.IsMiddleButtonPressed)
                {
                    lowWord |= (ushort)MODIFIERKEYS_FLAGS.MK_MBUTTON;
                }
                if (properties.IsXButton1Pressed)
                {
                    lowWord |= (ushort)MODIFIERKEYS_FLAGS.MK_XBUTTON1;
                }
                if (properties.IsXButton2Pressed)
                {
                    lowWord |= (ushort)MODIFIERKEYS_FLAGS.MK_XBUTTON2;
                }

                // Mouse wheel : https://docs.microsoft.com/en-us/windows/desktop/inputdev/wm-mousewheel
                if (message == PInvoke.WM_MOUSEWHEEL || message == PInvoke.WM_MOUSEHWHEEL)
                {
                    // TODO_WebView2 : See if this needs to be multiplied with scale for different dpi scenarios
                    highWord = (ushort)properties.MouseWheelDelta;
                }
                else if (message == PInvoke.WM_XBUTTONDOWN || message == PInvoke.WM_XBUTTONUP)
                    unchecked
                    {
                        // highWord Specifies which of the two XButtons is referenced by the message
                        Windows.UI.Input.PointerUpdateKind pointerUpdateKind = properties.PointerUpdateKind;
                        if (pointerUpdateKind == PointerUpdateKind.XButton1Pressed ||
                            pointerUpdateKind == PointerUpdateKind.XButton1Released)
                        {
                            highWord |= (ushort)MOUSEHOOKSTRUCTEX_MOUSE_DATA.XBUTTON1;
                        }
                        else if (pointerUpdateKind == PointerUpdateKind.XButton2Pressed ||
                                 pointerUpdateKind == PointerUpdateKind.XButton2Released)
                        {
                            highWord |= (ushort)MOUSEHOOKSTRUCTEX_MOUSE_DATA.XBUTTON2;
                        }
                    }

                WPARAM wParam = MakeWParam(lowWord, highWord);
                return wParam;
            }
        }

        static uint MakeWParam(ushort low, ushort high)
        {
            return ((uint)high << 16) | (uint)low;
        }
        static int MAKELPARAM(int LoWord, int HiWord)
        {
            return ((HiWord << 16) | (LoWord & 0xffff));
        }

        void OnXamlPointerMessage(
        uint message,
        PointerRoutedEventArgs args)
        {
            // Set Handled to prevent ancestor actions such as ScrollViewer taking focus on PointerPressed/PointerReleased.
            args.Handled = true;

            if (m_coreWebView is null || m_coreWebViewCompositionController is null)
            {
                // returning only because one can click within webview2 element even before it gets loaded
                // in such scenarios, the input gets ignored
                return;
            }

            PointerPoint logicalPointerPoint = args.GetCurrentPoint(this);
            Point logicalPoint = logicalPointerPoint.Position;
            Point physicalPoint = new(logicalPoint.X * m_rasterizationScale, logicalPoint.Y * m_rasterizationScale);
            PointerDeviceType deviceType = args.Pointer.PointerDeviceType;

            if (deviceType == PointerDeviceType.Mouse)
            {
                if (message == PInvoke.WM_MOUSELEAVE)
                {
                    //Debug.WriteLine($"Message: {(CoreWebView2MouseEventKind)message} VirtualKey: {CoreWebView2MouseEventVirtualKeys.None} Mouse Data: {0} Position: {default(Point)}");
                    m_coreWebViewCompositionController.SendMouseInput(
                        (CoreWebView2MouseEventKind)message,
                        CoreWebView2MouseEventVirtualKeys.None,
                    0,
                    new Point(0, 0));
                }
                else
                {
                    LPARAM l_param = WebView2Utility.PackIntoWin32StylePointerArgs_lparam(message, args, physicalPoint);
                    WPARAM w_param = WebView2Utility.PackIntoWin32StyleMouseArgs_wparam(message, args, logicalPointerPoint);

                    Point coords_win32 = new((short)LOWORD(l_param), (short)HIWORD(l_param));

                    Point coords = coords_win32;

                    // mouse data is nonzero for mouse wheel scrolling and XBUTTON events
                    uint mouse_data = 0;
                    if (message == PInvoke.WM_MOUSEWHEEL || message == PInvoke.WM_MOUSEHWHEEL)
                    {
                        mouse_data = (uint)GET_WHEEL_DELTA_WPARAM(w_param);
                    }
                    else if (message == PInvoke.WM_XBUTTONDOWN || message == PInvoke.WM_XBUTTONUP || message == PInvoke.WM_XBUTTONDBLCLK)
                    {
                        mouse_data = (uint)GET_XBUTTON_WPARAM(w_param);
                    }
                    //Debug.WriteLine($"Message: {(CoreWebView2MouseEventKind)message} VirtualKey: {(CoreWebView2MouseEventVirtualKeys)GET_KEYSTATE_WPARAM(w_param)} Mouse Data: {mouse_data} Position: {coords}");
                    m_coreWebViewCompositionController.SendMouseInput(
                        (CoreWebView2MouseEventKind)message,
                    (CoreWebView2MouseEventVirtualKeys)GET_KEYSTATE_WPARAM(w_param),
                    mouse_data,
                    coords);
                }
            }
            else if ((deviceType == Windows.Devices.Input.PointerDeviceType.Touch) ||
                     (deviceType == Windows.Devices.Input.PointerDeviceType.Pen))
            {
                PointerPoint inputPt = args.GetCurrentPoint(this);
                CoreWebView2PointerInfo outputPt = m_coreWebViewEnvironment!.CreateCoreWebView2PointerInfo();

                //PEN INPUT
                if (deviceType == PointerDeviceType.Pen)
                {
                    FillPointerPenInfo(inputPt, outputPt);
                    //Debug.WriteLine(inputPt.Properties.Pressure);
                    //Debug.WriteLine(outputPt.PenPressure);
                }

                //TOUCH INPUT
                if (deviceType == PointerDeviceType.Touch)
                {
                    FillPointerTouchInfo(inputPt, outputPt);
                }

                //GENERAL POINTER INPUT
                FillPointerInfo(inputPt, outputPt, args);

                m_coreWebViewCompositionController.SendPointerInput((CoreWebView2PointerEventKind)message, outputPt);
            }
        }
        static long HIWORD(LPARAM Number)
        {

            return (Number >> 16) & 0xffff;

        }

        static long LOWORD(LPARAM Number)
            => Number & 0xffff;
        static ulong HIWORD(WPARAM Number)
            => (Number >> 16) & 0xffff;
        static ulong LOWORD(WPARAM Number)
            => Number & 0xffff;

        static short GET_WHEEL_DELTA_WPARAM(WPARAM wParam)
            => (short)HIWORD(wParam);
        static short GET_XBUTTON_WPARAM(WPARAM wParam)
            => (short)HIWORD(wParam);
        static short GET_KEYSTATE_WPARAM(WPARAM wParam)
            => (short)LOWORD(wParam);
        // The transform is not available in matrix form outside core windows so needed
        // information about the transformation needs to be reconstructed by applying
        // the transform directly to a known set of points.
        // It is assumed that no shear transform is applied and currently rotation is not supported.
        Matrix4x4 GetMatrixFromTransform()
        {
            // Calculate transformation assuming 2D only.
            // Calculate transformed values
            var generalTransform = TransformToVisual(null);
            Point initialOrigin = new(0, 0);
            Point translatedOrigin = generalTransform.TransformPoint(initialOrigin);

            Matrix4x4 outputMatrix = new()
            {
                // Assign rotation
                M12 = 0.0f,
                M13 = 0.0f,
                M21 = 0.0f,
                M23 = 0.0f,
                M31 = 0.0f,
                M32 = 0.0f,

                // Assign offsets/translation
                // This should be the global physical pixel offset to the top left corner of the XAML HWND.
                M41 = (float)(translatedOrigin.X * m_rasterizationScale), // X offset
                M42 = (float)(translatedOrigin.Y * m_rasterizationScale), // Y offset
                M43 = 0.0f, // Z offset

                // Assign scale values
                // These values will just be 1.0 because Anaheim is getting their values in physical pixels,
                // so they don't need to do any extra unscaling.
                M11 = 1.0f, // X Scale
                M22 = 1.0f, // Y Scale
                M33 = 1.0f, // Z scale

                // Set to 0 (3D coordinate transform values)
                M14 = 0.0f,
                M24 = 0.0f,
                M34 = 0.0f,
                // Set to 1 to maintain non-zero det.
                M44 = 1.0f
            };

            return outputMatrix;
        }

        void ResetMouseInputState()
        {
            m_isLeftMouseButtonPressed = false;
            m_isMiddleMouseButtonPressed = false;
            m_isRightMouseButtonPressed = false;
            m_isXButton1Pressed = false;
            m_isXButton2Pressed = false;
        }
        (CoreWebView2MoveFocusReason m_storedMoveFocusReason, bool m_isPending) m_xamlFocusChangeInfo = (default, default);
        void HandleGotFocus(object sender, RoutedEventArgs e)
        {
            if (m_coreWebView != null && m_xamlFocusChangeInfo.m_isPending)
            {
                MoveFocusIntoCoreWebView(m_xamlFocusChangeInfo.m_storedMoveFocusReason);
                m_xamlFocusChangeInfo.m_isPending = false;
            }
        }
        void HandleGettingFocus(object sender, GettingFocusEventArgs args)
        {
            if (m_coreWebView != null)
            {
                CoreWebView2MoveFocusReason moveFocusReason = CoreWebView2MoveFocusReason.Programmatic;

                if (args.InputDevice == FocusInputDeviceKind.Keyboard)
                {
                    if (args.Direction == FocusNavigationDirection.Next)
                    {
                        moveFocusReason = CoreWebView2MoveFocusReason.Next;
                    }
                    else if (args.Direction == FocusNavigationDirection.Previous)
                    {
                        moveFocusReason = CoreWebView2MoveFocusReason.Previous;
                    }
                }

                m_xamlFocusChangeInfo.m_storedMoveFocusReason = moveFocusReason;
                m_xamlFocusChangeInfo.m_isPending = true;
            }
        }
        bool m_webHasFocus;
        void MoveFocusIntoCoreWebView(CoreWebView2MoveFocusReason reason)
        {
            try
            {
                m_coreWebViewController!.MoveFocus(reason);
                m_webHasFocus = true;
            }
            catch (COMException)
            {
                // Occasionally, a request to restore the minimized window does not complete. This triggers
                // FocusManager to set Xaml Focus to WV2 and consequently into CWV2 MoveFocus() call above, 
                // which in turn will attempt ::SetFocus() on InputHWND, and that will fail with E_INVALIDARG
                // since that HWND remains minimized. Work around by ignoring this error here. Since the app
                // is minimized, focus state is not relevant - the next (successful) attempt to restrore the app
                // will set focus into WV2/CWV2 correctly.
                Debugger.Break();
                //if (e.ErrorCode != 0x80070057) // E_INVALIDARG
                //{
                //    throw;
                //}
            }
        }

        // Since WebView takes HWND focus (via OnGotFocus -> MoveFocus) Xaml assumes
        // focus was lost for an external reason. When the next unhandled TAB KeyDown
        // reaches the XamlRoot element, Xaml's FocusManager will try to move focus to the next
        // Xaml control and force HWND focus back to itself, popping Xaml focus out of the
        // WebView2 control. We mark TAB handled in our KeyDown handler so that it is ignored
        // by XamlRoot's tab processing.
        // If the WebView2 has been closed, then we should let Xaml's tab processing handle it.
        void HandleKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Tab && !m_isClosed)
            {
                e.Handled = true;
            }
        }

        // When Win32 HWND focus is switched to InputWindow, VK_TAB's processed by Xaml's CoreWindow
        // hosting accelerator key handling do not get dispatched to the child InputWindow.
        // Send CoreWebView2 the missing Tab/KeyDown so that tab handling occurs in Anaheim.
        void HandleAcceleratorKeyActivated(CoreDispatcher coreDispatcher, AcceleratorKeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.Tab &&
                args.EventType == CoreAcceleratorKeyEventType.KeyDown &&
                m_webHasFocus &&
                args.Handled)
            {
                uint message = PInvoke.WM_KEYDOWN;
                WPARAM wparam = new((nuint)VIRTUAL_KEY.VK_TAB);
                LPARAM lparam = MAKELPARAM(0x0001, 0x000f);  // flags copied from matching WM_KEYDOWN

                LRESULT result = new(SendMessage(GetActiveInputWindowHwnd(), message, wparam, lparam));
                if (result == 0)
                {
                    //Debugger.Break();
                    //winrt::check_hresult(HRESULT_FROM_WIN32(::GetLastError()));
                }
            }
        }

        void RegisterXamlEventHandlers()
        {
            GettingFocus += HandleGettingFocus;
            GotFocus += HandleGotFocus;

            PointerPressed += HandlePointerPressed;
            PointerReleased += HandlePointerReleased;
            PointerMoved += HandlePointerMoved;
            PointerWheelChanged += HandlePointerWheelChanged;
            PointerExited += HandlePointerExited;
            PointerEntered += HandlePointerEntered;
            PointerCanceled += HandlePointerCanceled;
            PointerCaptureLost += HandlePointerCaptureLost;
            KeyDown += HandleKeyDown;

            var coreWindow = CoreWindow.GetForCurrentThread();
            // TODO: We do not have direct analogue for AcceleratorKeyActivated with DispatcherQueue in Islands/ win32. Please refer Task# 30013704 for  more details.
            if (coreWindow != null)
            {
                Dispatcher.AcceleratorKeyActivated += HandleAcceleratorKeyActivated;
            }

            SizeChanged += HandleSizeChanged;

        }

        private void CoreWebView2CursorChanged(CoreWebView2CompositionController sender, object args)
        {
            if (m_isPointerOver && m_coreWebViewCompositionController is not null)
                CoreWindow.GetForCurrentThread().PointerCursor = m_coreWebViewCompositionController.Cursor;
        }

        void UnregisterXamlEventHandlers()
        {
            GettingFocus -= HandleGettingFocus;
            GotFocus -= HandleGotFocus;

            PointerPressed -= HandlePointerPressed;
            PointerReleased -= HandlePointerReleased;
            PointerMoved -= HandlePointerMoved;
            PointerWheelChanged -= HandlePointerWheelChanged;
            PointerExited -= HandlePointerExited;
            PointerEntered -= HandlePointerEntered;
            PointerCanceled -= HandlePointerCanceled;
            PointerCaptureLost -= HandlePointerCaptureLost;
            KeyDown -= HandleKeyDown;

            var coreWindow = CoreWindow.GetForCurrentThread();
            if (coreWindow != null)
            {
                Dispatcher.AcceleratorKeyActivated -= HandleAcceleratorKeyActivated;
            }

            SizeChanged -= HandleSizeChanged;
        }
        //varmationPeer OnCreatevarmationPeer()
        //{
        //    return winrt::make<WebView2varmationPeer>(*this);
        //}
        TaskCompletionSource<bool>? m_creationInProgressAsync;


        public async Task EnsureCoreWebView2Async()
        {
            // If CWV2 exists already, return immediately/synchronously
            if (m_coreWebView != null)
            {
                Debug.Assert(m_coreWebViewEnvironment != null && m_coreWebViewController != null);
                return;
            }

            // If CWV2 is being created, return when the pending creation completes
            if (m_creationInProgressAsync != null)
            {
                Debug.Assert(m_isExplicitCreationInProgress || m_isImplicitCreationInProgress);
                await m_creationInProgressAsync.Task;
            }

            // Otherwise, kick off a new CWV2 creation
            else
            {
                m_isExplicitCreationInProgress = true;
                await CreateCoreObjects();
            }

            Debug.Assert(!m_isImplicitCreationInProgress && !m_isExplicitCreationInProgress);
        }

        void DisconnectFromRootVisualTarget()
        {
            if (m_coreWebViewCompositionController is not null)
            {
                m_coreWebViewCompositionController.RootVisualTarget = null;
                //var coreWebView2CompositionControllerInterop = (ICoreWebView2CompositionControllerInterop)m_coreWebViewCompositionController;
                //winrt::check_hresult(coreWebView2CompositionControllerInterop->put_RootVisualTarget(nullptr));
            }
        }
        void TryCompleteInitialization()
        {
            // If proper Anaheim not present, no further initialization is necessary
            if (m_shouldShowMissingAnaheimWarning)
            {
                return;
            }

            // If called directly after CWV2 creation completes, it's possible Xaml WV2 Loaded event did not yet fire.
            // Skip  work for now - this will be called again when WV2 Loaded fires.
            if (!SafeIsLoaded())
            {
                return;
            }

            // If called from Loaded handler, it's possible CWV2 is not ready yet.
            // Skip work for now - this will be called again when CWV2 is ready.
            if (m_coreWebView == null)
            {
                return;
            }

            // In a non-CoreWindow scenario, we may have created the CoreWebView2 with a dummy hwnd as its parent
            // (see EnsureTemporaryHostHwnd()), in which case we need to update to use the real parent here.
            // If we used a CoreWindow parent, that hwnd has not changed. The CoreWebView2 does not allow us to switch
            // from using the CoreWindow as the parent to the XamlRoot.
            CoreWindow coreWindow = CoreWindow.GetForCurrentThread();
            if (coreWindow == null)
            {
                HWND prevParentWindow = m_xamlHostHwnd;
                m_xamlHostHwnd = default;
                HWND newParentWindow = GetHostHwnd();
                UpdateParentWindow(newParentWindow);
            }

            XamlRootChangedHelper(true);
            var xamlRoot = XamlRoot;
            if (xamlRoot != null)
            {
                xamlRoot.Changed += XamlRootChangedHanlder;
            }
            else
            {
                Window.Current.VisibilityChanged += VisiblityChangedHandler;
            }

            if (!m_actualThemeChangedRegistered)
            {
                ActualThemeChanged += delegate { UpdateDefaultVisualBackgroundColor(); };
                m_actualThemeChangedRegistered = true;
            }

            // TODO_WebView2: Currently, AccessibilitySettings.HighContrastChanged does not work without a core window, and throws
            // an ElementNotFound exception. VS default behavior is to break on exceptions, regardless of whether we catch it below.
            // This is not a good experience for developers, since this happens on every WebView2 creation, and it feels like an error
            // even though it's expected.
            // We should avoid this altogether on desktop by not registering for the HighContrastChanged event, since for now it
            // will never be raised. Once Task #24777629 is fixed, we can remove the coreWindow check.
            if (!m_highContrastChangedRegistered && coreWindow != null)
            {
                AccessibilitySettings.HighContrastChanged += delegate { UpdateDefaultVisualBackgroundColor(); };
                m_highContrastChangedRegistered = true;
            }

            // WebView2 in WinUI 2 is a ContentControl that either renders its web content to a SpriteVisual, or in the case that
            // the WebView2 Runtime is not installed, renders a message to that effect as its Content. In the case where the
            // WebView2 starts with Visibility.Collapsed, hit testing code has trouble seeing the WebView2 if it does not have
            // Content. To work around this, give the WebView2 a transparent Grid as Content that hit testing can find. The size
            // of this Grid must be kept in sync with the size of the WebView2 (see ResizeChildPanel()).
            AddChildPanel();

            CreateAndSetVisual();

            // If we were recreating the webview after a core process failure, indicate that we have now recovered
            m_isCoreFailure_BrowserExited_State = false;
        }
        bool m_actualThemeChangedRegistered = false;
        bool m_highContrastChangedRegistered = false;
        void VisiblityChangedHandler(object sender, VisibilityChangedEventArgs e)
        {
            HandleXamlRootChanged();
        }
        void XamlRootChangedHanlder(XamlRoot sender, XamlRootChangedEventArgs args)
        {
            HandleXamlRootChanged();
        }
        void AddChildPanel()
        {
            Content = new Grid { Background = new SolidColorBrush(Colors.Transparent) };
        }
        SpriteVisual? m_visual;
        void CreateAndSetVisual()
        {
            m_visual ??= Window.Current.Compositor.CreateSpriteVisual();
            UpdateDefaultVisualBackgroundColor();

            SetCoreWebViewAndVisualSize((float)ActualWidth, (float)ActualHeight);

            ElementCompositionPreview.SetElementChildVisual(this, m_visual);

            m_coreWebViewCompositionController!.RootVisualTarget = m_visual;
        }

        void UpdateParentWindow(HWND newParentWindow)
        {
            if (m_tempHostHwnd != default && m_coreWebViewController != null)
            {
                var windowRef = CoreWebView2ControllerWindowReference.CreateFromWindowHandle((ulong)newParentWindow.Value);

                // Reparent webview host
                m_coreWebViewController.ParentWindow = windowRef;

                DestroyWindow(m_tempHostHwnd);
                m_tempHostHwnd = default;
            }
        }

        bool SafeIsLoaded()
        {
            return IsLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs args)
        {
            //m_loaded = true; // Track for downlevel OS

            // OnLoaded and OnUnloaded are fired from XAML asynchronously, and unfortunately they could
            // be out of order.  If the element is removed from tree A and added to tree B, we could get
            // a Loaded event for tree B *before* we see the Unloaded event for tree A.  To handle this:
            //  * When we get a Loaded/Unloaded event, check the IsLoaded property. If it doesn't match
            //      the event we're in, nothing needs to be done since the other handler took care of it.
            //  * When we see a Loaded event when we have been or are already loaded, remove the
            //      XamlRootChanged event handler for the old tree and bind to the new one.

            // If we're not loaded, there's nothing for us to do since Unloaded took care of everything
            if (!SafeIsLoaded())
            {
                return;
            }

            TryCompleteInitialization();

            if (VisualTreeHelper.GetChildrenCount(this) > 0)
            {
                var contentPresenter = VisualTreeHelper.GetChild(this, 0) as ContentPresenter;
                if (contentPresenter is not null)
                {
                    contentPresenter.Background = Background;
                    contentPresenter.HorizontalAlignment = HorizontalAlignment.Stretch;
                    contentPresenter.VerticalAlignment = VerticalAlignment.Stretch;
                    contentPresenter.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                    contentPresenter.VerticalContentAlignment = VerticalAlignment.Stretch;
                }
            }
        }

        void OnUnloaded(object sender, RoutedEventArgs args)
        {
            // m_loaded = false;

            // Note that we always check the IsLoaded property before doing unloading actions, since we
            // could get an Unloaded event for an old tree A *after* a Loaded event for a new tree B.
            // See comment in WebView2::OnLoaded
            if (SafeIsLoaded())
            {
                return;
            }

            UpdateRenderedSubscriptionAndVisibility();

            var xamlRoot = XamlRoot;
            if (xamlRoot != null)
            {
                xamlRoot.Changed -= XamlRootChangedHanlder;
            }
            Window.Current.VisibilityChanged -= VisiblityChangedHandler;

            DisconnectFromRootVisualTarget();
        }
        bool m_isHostVisible;
        void XamlRootChangedHelper(bool forceUpdate)
        {
            var (scale, hostVisibility) = new Func<(double, bool)>(delegate
            {
                var xamlRoot = XamlRoot;
                if (xamlRoot != null)
                {
                    var scale = (float)xamlRoot.RasterizationScale;
                    bool hostVisibility = xamlRoot.IsHostVisible;

                    return (scale, hostVisibility);
                }

                double rawPixelsPerViewPixel = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

                return (rawPixelsPerViewPixel, Window.Current.Visible);
            })();


            if (forceUpdate || (scale != m_rasterizationScale))
            {
                m_rasterizationScale = scale;
                m_isHostVisible = hostVisibility; // If we did forceUpdate we'll want to update host visibility here too
                if (m_coreWebViewController != null)
                {
                    m_coreWebViewController.RasterizationScale = scale;
                }
                SetCoreWebViewAndVisualSize((float)ActualWidth, (float)ActualHeight);
                CheckAndUpdateWebViewPosition();
                UpdateRenderedSubscriptionAndVisibility();
            }
            else if (hostVisibility != m_isHostVisible)
            {
                m_isHostVisible = hostVisibility;
                CheckAndUpdateVisibility();
            }
        }

        void HandleXamlRootChanged()
        {
            XamlRootChangedHelper(false);
        }

        void HandleSizeChanged(object sender, SizeChangedEventArgs args)
        {
            SetCoreWebViewAndVisualSize((float)args.NewSize.Width, (float)args.NewSize.Height);
        }

        void HandleRendered(object sender, object args)
        {
            if (m_coreWebView != null)
            {
                // Check if the position of the WebView inside the app has changed
                CheckAndUpdateWebViewPosition();
                // Check if the position of the window itself has changed
                CheckAndUpdateWindowPosition();
                // Check if the visibility property of a parent element has changed
                CheckAndUpdateVisibility();
            }
        }
        Point m_webViewScaledPosition, m_webViewScaledSize;
        void CheckAndUpdateWebViewPosition()
        {
            if (m_coreWebViewController == null)
            {
                return;
            }

            // Skip this work if WebView2 has just been removed from the tree - otherwise the CWV2.Bounds update could cause a flicker.
            //
            // After WebView2 is removed from the tree, this handler gets run one more time during the frame's render pass 
            // (WebView2::HandleRendered()). The removed element's ActualWidth or ActualHeight could now evaluate to zero 
            // (if Width or Height weren't explicitly set), causing 0-sized Bounds to get applied below and clear the web content, 
            // producing a flicker that last until DComp Commit for this frame is processed by the compositor.
            if (!SafeIsLoaded())
            {
                return;
            }

            // Check if the position of the WebView2 within the window has changed
            bool changed = false;
            var transform = TransformToVisual(null);
            var topLeft = transform.TransformPoint(new Point(0, 0));

            var scaledTopLeftX = Math.Ceiling(topLeft.X * m_rasterizationScale);
            var scaledTopLeftY = Math.Ceiling(topLeft.Y * m_rasterizationScale);

            if (scaledTopLeftX != m_webViewScaledPosition.X || scaledTopLeftY != m_webViewScaledPosition.Y)
            {
                m_webViewScaledPosition.X = scaledTopLeftX;
                m_webViewScaledPosition.Y = scaledTopLeftY;
                changed = true;
            }

            var scaledSizeX = Math.Ceiling(ActualWidth * m_rasterizationScale);
            var scaledSizeY = Math.Ceiling(ActualHeight * m_rasterizationScale);
            if (scaledSizeX != m_webViewScaledSize.X || scaledSizeY != m_webViewScaledSize.Y)
            {
                m_webViewScaledSize.X = scaledSizeX;
                m_webViewScaledSize.Y = scaledSizeY;
                changed = true;
            }

            if (changed)
            {
                // We create the Bounds using X, Y, width, and height
                m_coreWebViewController.Bounds = new Rect(
                    (m_webViewScaledPosition.X),
                    (m_webViewScaledPosition.Y),
                    (m_webViewScaledSize.X),
                    (m_webViewScaledSize.Y)
                );
            }
        }

        Rect GetBoundingRectangle()
        {
            return new Rect(
                (m_webViewScaledPosition.X),
            (m_webViewScaledPosition.Y),
            (m_webViewScaledSize.X),
            (m_webViewScaledSize.Y));
        }

        void SetCoreWebViewAndVisualSize(float width, float height)
        {
            if (m_coreWebView == null && m_visual == null) return;

            if (m_coreWebView != null)
            {
                CheckAndUpdateWebViewPosition();
            }

            // The CoreWebView2 visuals hosted under the bridge visual are already scaled for the rasterization scale.
            // To keep them from being scaled again from the scale above the WebView2 element, we need to apply
            // an inverse scale on the bridge visual. Since the inverse scale will reduce the size of the bridge visual, we
            // need to scale up the size by the rasterization scale to compensate.

            if (m_visual != null)
            {
                float m_rasterizationScale = (float)this.m_rasterizationScale;
                Vector2 newSize = new(width * m_rasterizationScale, height * m_rasterizationScale);
                Vector3 newScale = new(1.0f / m_rasterizationScale, 1.0f / m_rasterizationScale, 1.0f);

                m_visual.Size = newSize;
                m_visual.Scale = newScale;
            }
        }
        Point m_hostWindowPosition;
        void CheckAndUpdateWindowPosition()
        {
            var hostWindow = GetHostHwnd();
            if (hostWindow == null)
            {
                return;
            }

            SysPoint windowPosition = new(0, 0);
            ClientToScreen(hostWindow, ref windowPosition);
            if (m_hostWindowPosition.X != windowPosition.X || m_hostWindowPosition.Y != windowPosition.Y)
            {
                m_hostWindowPosition.X = windowPosition.X;
                m_hostWindowPosition.Y = windowPosition.Y;

                if (m_coreWebViewController != null)
                {
                    m_coreWebViewController.NotifyParentWindowPositionChanged();
                }
            }
        }
        bool m_isVisible;
        void CheckAndUpdateVisibility(bool force = false)
        {
            // Keep booleans in this order to prevent doing expensive tree walk if we don't have to.
            bool currentVisibility = Visibility == Visibility.Visible &&
                                     SafeIsLoaded() &&
                                     m_isHostVisible &&
                                     AreAllAncestorsVisible();
            if (m_isVisible != currentVisibility || force)
            {
                m_isVisible = currentVisibility;

                UpdateCoreWebViewVisibility();
            }
        }
        static class SharedHelpers
        {
            public static void ScheduleActionAfterWait(CoreDispatcher Dispatcher,
        Action action,
        uint millisecondWait)
            {
                // The callback that is given to CreateTimer is called off of the UI thread.
                // In order to make this useful by making it so we can interact with XAML objects,
                // we'll use the dispatcher to first post our work to the UI thread before executing it.
                var timer = ThreadPoolTimer.CreateTimer(async _
                    => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action()),
                    TimeSpan.FromMilliseconds(millisecondWait)
                );
            }
        }
        // When we hide the CoreWebView too early, we would see a flash caused by SystemVisualBridge's BackgroundColor being displayed.
        // To resolve this, we delay the call to hide CoreWV if the WebView is being hidden.
        void UpdateCoreWebViewVisibility()
        {
            void updateCoreWebViewVisibilityAction()
            {
                if (m_coreWebViewController != null)
                {
                    m_coreWebViewController.IsVisible = m_isVisible;
                }
            }

            if (!m_isVisible && m_isHostVisible)
            {
                SharedHelpers.ScheduleActionAfterWait(Dispatcher, updateCoreWebViewVisibilityAction, 200);
            }
            else
            {
                if (m_coreWebViewController != null)
                {
                    m_coreWebViewController.IsVisible = m_isVisible;
                }
            }
        }

        bool AreAllAncestorsVisible()
        {
            bool allAncestorsVisible = true;
            DependencyObject? parentAsDO = Parent;
            while (parentAsDO != null)
            {
                UIElement parentAsUIE = (UIElement)parentAsDO;
                Visibility parentVisibility = parentAsUIE.Visibility;
                if (parentVisibility == Visibility.Collapsed)
                {
                    allAncestorsVisible = false;
                    break;
                }
                parentAsDO = VisualTreeHelper.GetParent(parentAsDO);
            }

            return allAncestorsVisible;
        }
        void UpdateRenderedSubscriptionAndVisibility()
        {
            // The Rendered subscription is turned off for better performance when this element is hidden, or not loaded.
            // However, when this element is effectively hidden due to an ancestor being hidden, we should still subscribe --
            // otherwise, if the ancestor becomes visible again, we won't have the check in HandleRendered to inform us.
            if (SafeIsLoaded() && Visibility == Visibility.Visible)
            {
                if (!m_renderedRegistered)
                {
                    Windows.UI.Xaml.Media.CompositionTarget.Rendered += HandleRendered;
                    m_renderedRegistered = true;
                }
            }
            else
            {
                Windows.UI.Xaml.Media.CompositionTarget.Rendered -= HandleRendered;
                m_renderedRegistered = false;
            }
            CheckAndUpdateVisibility(true);
        }
        bool m_renderedRegistered = false;
        // Do not reset Source property as it be useful to app developer for restoring state
        void ResetProperties()
        {
            //SetCanGoForward(false);
            //SetCanGoBack(false);
        }
    }
}
