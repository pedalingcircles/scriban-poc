# Overview

This document explains how artifacts in this repo are versioned. 

## Nerdbank.GitVersioning

This is a tool supported by the .NET foundation and supports Semantic versioning approach used
to version dotnet assemblies in this repo. It uses Git as a basic method to support this.

## Setup

Navigate to the [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) repo an read the documentation.

Install the nbgv tool by running 

`dotnet tool install -g nbgv`

> Check the command line as you might have to update your `PATH` to include the dotnet tools in your profile
> 
> `export PATH="$PATH:/Users/mike/.dotnet/tools"`

Check if installed with `nbgv --version`

## Utilities

The following utilities might be useful in checking for file metadata

On a Mac you can use the `files` command to get to generic file meta data

`file MyLibrary.dll`

This will output

`Byvrate.FileProcessing.Core.dll: PE32 executable (DLL) (console) Intel 80386 Mono/.Net assembly, for MS Windows`

Using `strings` will extract more information

`strings Byvrate.FileProcessing.Core.dll`

But that oupput might be a little raw. A better approach might be 

`strings Byvrate.FileProcessing.Core.dll | grep -i version`

example output

```
AssemblyFileVersionAttribute
AssemblyInformationalVersionAttribute
System.Runtime.Versioning
AssemblyFileVersion
AssemblyInformationalVersion
AssemblyVersion
.NETCoreApp,Version=v9.0
Nerdbank.GitVersioning.Tasks
```

