# FFF
Fast Find in Files

Fast find in file is a command line utility that uses asynchronous and concurrecny capability to crawl local files for specific string as fast as possible.
Goal is to concentrate all computing power into searching the string, in order to report results as fast as possible.
Th command line tool usage is described with `fff --help`

```
fff:
  Fast Search in Files

Usage:
  fff [options] <search>

Arguments:
  <search>    string to search for

Options:
  -p, --path <path>      path where to search [default: /home/felice/FFF]
  -f, --files <files>    use double quotes to avoid wildcard expansion, ie "*.cpp" [default: *.*]
  -i, --ignore-case      ignore case [default: False]
  -x, --use-regex        use search string as a pattern [default: False]
  -n, --name-only        just look for file names [default: False]
  -j, --json             output in json without coloring [default: False]
  --version              Show version information
  -?, -h, --help         Show help and usage information
```

## VSCode ADDIN
Providing that fff is available to launch on your system ( ie install in path or /usr/local/bin) there is an available extension you can download from the Release, or in the path src/vscode-addin/fff/fff-x.x.x.vsix, exposing the command FFF. This allow you to search and point and click to go to the found results.
This is a fast search alternative to the one provided by vscode itself.

![image](https://user-images.githubusercontent.com/73569/149669270-8d389d38-c0aa-43ba-86eb-c2303a6dc504.png)

In order to package the vsaddin you need to install node and python,  and follow these steps:
```
npm install -g vsce 
```
then move in the folder src\vscode-addin\fff
and run 
```
npm install
vsce package
```
This will produce a vsix with the version according to what is in package.json. 
The vsix can then be installed in vscode by the menu Extensions/Triple Dot/Install from vsix...


## tool build
In order to build the toolin a single file working without dependencies:


- Win64: dotnet publish -r win-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesInSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -c release

## efficency
By using concurrency and multithreading all resources are optimized to make search as fast as possible.
Here below an example CPU usage during a search:
![image](https://user-images.githubusercontent.com/73569/112138701-20865680-8bd2-11eb-921a-aaa921bc5852.png)