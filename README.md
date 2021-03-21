# FFF
Fast Find in Files

Fast find in file is a command line utility that uses asynchronous and concurrecny capability to crawl local files for specific string as fast as possible.
Goal is to conncentrate all computing power into searching the string, in order to report results as fast as possible.
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
  --version              Show version information
  -?, -h, --help         Show help and usage information
```
