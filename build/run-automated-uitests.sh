﻿#!/bin/bash
set -euo pipefail
IFS=$'\n\t'

cd $BUILD_SOURCESDIRECTORY/build/wasm-uitest-binaries

npm i chromedriver@83.0.1
npm i puppeteer@3.1.0
mono $BUILD_SOURCESDIRECTORY/build/nuget/NuGet.exe install NUnit.ConsoleRunner -Version 3.10.0

export UNO_UITEST_TARGETURI=http://localhost:8000
export UNO_UITEST_DRIVERPATH_CHROME=$BUILD_SOURCESDIRECTORY/build/wasm-uitest-binaries/node_modules/chromedriver/lib/chromedriver
export UNO_UITEST_CHROME_BINARY_PATH=$BUILD_SOURCESDIRECTORY/build/wasm-uitest-binaries/node_modules/puppeteer/.local-chromium/linux-756035/chrome-linux/chrome
export UNO_UITEST_SCREENSHOT_PATH=$BUILD_ARTIFACTSTAGINGDIRECTORY/screenshots/wasm-automated-$DOTNET_WASM_RUNTIME
export UNO_UITEST_PLATFORM=Browser

mkdir -p $UNO_UITEST_SCREENSHOT_PATH

## The python server serves the current working directory, and may be changed by the nunit runner
bash -c "cd $BUILD_SOURCESDIRECTORY/build/wasm-uitest-binaries/site-$DOTNET_WASM_RUNTIME; python server.py &"

export TEST_FILTERS="namespace != 'SamplesApp.UITests.Snap'"

mono $BUILD_SOURCESDIRECTORY/build/wasm-uitest-binaries/NUnit.ConsoleRunner.3.10.0/tools/nunit3-console.exe \
    --where "$TEST_FILTERS" \
    $BUILD_SOURCESDIRECTORY/build/wasm-uitest-binaries/test-bin/SamplesApp.UITests.dll
