#pragma once
#include <vector>
#include <string>
#include <iostream>

class Errors
{
public:
    Errors() = default;

    void AddError(const std::string& errorMessage)
    {
        errors.push_back(errorMessage);
    }

    bool HasErrors() const
    {
        return !errors.empty();
    }

    void PrintErrors() const
    {
        for (const auto& error : errors)
        {
            // Print or handle each error as needed
            // Here, we simply print them to the console
            std::cout << "Error: " << error << std::endl;
        }
    }

private:
    std::vector<std::string> errors;
};
