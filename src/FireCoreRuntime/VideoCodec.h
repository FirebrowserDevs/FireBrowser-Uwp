#pragma once

//inline int main() {
    HRESULT hr = CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);
    //if (SUCCEEDED(hr)) {
      //  hr = MFStartup(MF_VERSION);
        //if (SUCCEEDED(hr)) {
            //IMFTransform* pEncoder = nullptr;

            // Create an encoder for the desired codec (e.g., H.264).
            // You can replace CLSID_AVIMFVideoCompressor with the codec you want to use.
            //hr = CoCreateInstance(CLSID_AVIMFVideoCompressor, nullptr, CLSCTX_INPROC_SERVER, IID_IMFTransform, (void**)&pEncoder);

            //if (SUCCEEDED(hr)) {
                // Check if the GPU supports the codec.
                UINT32 ui32NumInputStreams, ui32NumOutputStreams;
               // hr = pEncoder->GetStreamCount(&ui32NumInputStreams, &ui32NumOutputStreams);
               // if (SUCCEEDED(hr)) {
                    // Check if the GPU supports the codec based on your criteria.
                   // bool gpuSupportsCodec = CheckGPUSupport(pEncoder);

                    //if (gpuSupportsCodec) {
                        // Use the GPU-accelerated codec.
                       // std::cout << "GPU supports the codec." << std::endl;
                    //}
                    //else {
                        // Use an alternative compression method for older GPUs.
                      //  std::cout << "GPU doesn't support the codec. Using alternative method." << std::endl;
                        // Implement your alternative compression method here.
                    //}
               // }

               // pEncoder->Release();
           // }

            //MFShutdown();
        //}

        //CoUninitialize();
   // }

    //return 0;
//}
