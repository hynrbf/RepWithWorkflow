# RepWithWorkflow

orignal ppeline in actions
name: Build and Deploy

on:
  push:
    branches:
      - main
      
  pull_request_target:
    types: [closed]
    branches:
      - main

env:
  AZURE_FUNCTIONAPP_NAME: 'function-api-testing'   # set this to your function app name on Azure
  AZURE_FUNCTIONAPP_PACKAGE_PATH: './CSharp.Functions/ComplyDexFunction'       # set this to the path to your function app project, defaults to the repository root
  DOTNET_VERSION: '6.0.x'                   # set this to the dotnet version to use (e.g. '2.1.x', '3.1.x', '5.0.x')
  
jobs:
  build:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v4
      name: Checkout code

    # Deploy the Front End  
    - name: Install Node.js
      uses: actions/setup-node@v3
      with:
        node-version: '18.16.0'

    - name: Clean dist folder
      run: rm -rf vue.jsonforms/dist/*
      working-directory: ${{ github.workspace }}

    - name: npm install and build
      run: |
        npm install
        npm i --save-dev @types/uuid
        npm run build
      working-directory: vue.jsonforms

    - name: Deploy to Azure Static Web App
      uses: Azure/static-web-apps-deploy@v1
      with:
        repo_token: ${{ secrets.GITHUB_TOKEN }}
        action: "upload"
        app_location: "/vue.jsonforms"
        output_location: "dist"
        api_token: '004f197aa05e11bf5748a5f0625388173d73e84313df2dab88e89927d4f4495e3-1e51d804-2793-4cf7-8ab7-56222d2748c900066332' #${{ secrets.DEPLOYMENT_TOKEN }}
        azure_static_web_apps_api_token: '004f197aa05e11bf5748a5f0625388173d73e84313df2dab88e89927d4f4495e3-1e51d804-2793-4cf7-8ab7-56222d2748c900066332' #${{ secrets.DEPLOYMENT_TOKEN }}
        
  build-and-deploy:
    runs-on: windows-latest
    environment: dev
    steps:
    - name: 'Checkout GitHub Action'
      uses: actions/checkout@v3

    - name: Setup DotNet ${{ env.DOTNET_VERSION }} Environment
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}

    - name: 'Resolve Project Dependencies Using Dotnet'
      shell: pwsh
      run: |
        pushd './${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}'
        dotnet build --configuration Release --output ./output
        popd

    - name: 'Run Azure Functions Action'
      uses: Azure/functions-action@v1
      id: fa
      with:
        app-name: ${{ env.AZURE_FUNCTIONAPP_NAME }}
        slot-name: 'production'
        package: '${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}/output'
        publish-profile: ${{ secrets.AZURE_FUNCTIONAPP_PUBLISH_PROFILE_API }}
