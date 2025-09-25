#!/bin/bash
# filepath: /home/lepton/Documents/git_projects/RobustFit/build.sh

set -e  # Exit on any error

echo "RobustFit Build Script"
echo "====================="

# Function to clean the project
clean_project() {
    echo "Cleaning project..."
    dotnet clean
    
    # Remove build artifacts
    echo "Removing build artifacts..."
    find . -type d -name "obj" -o -name "bin" | xargs rm -rf
    
    # Remove any assembly-related files that might cause conflicts
    find . -name "*Assembly*" -not -path "*/obj/*" -not -path "*/bin/*" -delete 2>/dev/null || true
    find . -name "*.AssemblyInfo.cs" -not -path "*/obj/*" -not -path "*/bin/*" -delete 2>/dev/null || true
    find . -name "*.AssemblyAttributes.cs" -not -path "*/obj/*" -not -path "*/bin/*" -delete 2>/dev/null || true
    
    echo "Clean completed."
}

# Function to build the project
build_project() {
    echo "Building RobustFit library..."
    dotnet build --configuration Release
    
    build_result=$?
    if [ $build_result -eq 0 ]; then
        echo "Build successful!"
    else
        echo "Build failed!"
        exit 1
    fi
    
    # Create NuGet package
    echo "Creating NuGet package..."
    if [ -d "src/RobustFit.Core" ]; then
        dotnet pack src/RobustFit.Core -c Release -o artifacts
    else
        echo "Warning: Could not find src/RobustFit.Core to create NuGet package"
    fi
}

# Function to run the demo
run_demo() {
    echo "Running RobustFit demo..."
    if [ -d "samples/RobustFit.Demo" ]; then
        dotnet run --project samples/RobustFit.Demo --configuration Release
    else
        echo "Demo project not found at samples/RobustFit.Demo"
        exit 1
    fi
}

# Function to run tests
run_tests() {
    if [ -d "tests/RobustFit.UnitTests" ]; then
        echo "Running tests..."
        dotnet test --configuration Release
    else
        echo "No test project found at tests/RobustFit.UnitTests"
        exit 1
    fi
}

# Function to create NuGet package only
create_nuget() {
    echo "Creating NuGet package only..."
    if [ -d "src/RobustFit.Core" ]; then
        dotnet pack src/RobustFit.Core -c Release -o artifacts
        if [ $? -eq 0 ]; then
            echo "NuGet package created successfully in artifacts directory"
        else
            echo "Failed to create NuGet package"
            exit 1
        fi
    else
        echo "Error: Could not find src/RobustFit.Core directory"
        exit 1
    fi
}

# Parse command line arguments
case "$1" in
    clean)
        clean_project
        ;;
    build)
        build_project
        ;;
    demo)
        build_project
        run_demo
        ;;
    test)
        build_project
        run_tests
        ;;
    nuget)
        create_nuget
        ;;
    all)
        clean_project
        build_project
        run_tests
        run_demo
        ;;
    *)
        echo "Usage: $0 {clean|build|demo|test|nuget|all}"
        echo ""
        echo "Commands:"
        echo "  clean  - Clean build artifacts"
        echo "  build  - Build the library"
        echo "  demo   - Build and run the demo"
        echo "  test   - Build and run tests"
        echo "  nuget  - Create NuGet package"
        echo "  all    - Clean, build, test, and run demo"
        exit 1
        ;;
esac

echo "Done!"
