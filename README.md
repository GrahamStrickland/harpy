# Harpy

## Overview

Harbour to C# transpiler with expression parsing based on the parser from Robert Nystrom's
["Pratt Parsers: Expression Parsing Made Easy"](https://journal.stuffwithstuff.com/2011/03/19/pratt-parsers-expression-parsing-made-easy/)
and other ideas from
[magpie](https://github.com/munificent/magpie/blob/master/src/com/stuffwithstuff/magpie).

## Requirements

Type inference is to be performed using the names of variables and whatever contextual information can
be used to find the intended type of a variable. This means that all variables should use 
[Hungarian notation](https://en.wikipedia.org/wiki/Hungarian\_notation) to indicate the type, e.g.,
`lTemporary` (boolean), `nNumber` (numeric), `cName` (string), etc.

All free functions which are not methods of some class will need to be placed into the "global" scope 
of a project, which is a partial class spread across every source file in the source directory passed
to Harpy. This may be mitigated by refactoring as much Harbour source code as possible to use the Class(y)
syntax, thereby avoiding the use of the "global" partial class. This syntax also allows for typing to
be inferred more accurately, e.g., `var cName as string`.

## Current State

Harpy currently only parses some parts of the Harbour grammar and can output the AST representation of the source code.

The goal of this project is to be able to generate such an AST representation and transform it into
a C# AST which will be used to generate C# source code via the
[Roslyn SDK](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/).

One major issue affecting this task is the extensible nature of Harbour's
[preprocessor directives](https://harbour.github.io/ng/c53g01c/ng11fd89.html), which can essentially be 
used to generate arbitrary syntax extensions. Ultimately Harpy will probably need its own preprocessor, 
but for now the aim is to treat the [Class(y)](https://harbour.github.io/ng/classy/menu.html) class 
definitions as source code instead of using a preprocessor, since that syntax is fairly well-defined.

Other preprocessor directives, like macros, may also end up being treated like source code until a better
plan for the preprocessor is created.
