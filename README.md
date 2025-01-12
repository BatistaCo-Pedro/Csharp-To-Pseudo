# C# to Pseudo Code Converter

Welcome to the **C# to Pseudo Code Converter** project! This tool aims to take C# code snippets and output a language-agnostic pseudo code version. Eventually, this converter will be integrated into [Gainliner.com](https://gainliner.com) to help visualize code logic and aid in learning or documentation.

> [!WARNING]
> **This project is currently in a very early stage and it does not work as intended** - please give it some time.

---

## Table of Contents
1. [Project Overview](#project-overview)
2. [Current Status](#current-status)
3. [How It Works](#how-it-works)
4. [Roadmap](#roadmap)

---

## Project Overview

The goal of this project is to:

- **Parse** basic C# code syntax (methods, classes, loops, conditionals, etc.).
- **Generate** a simplified pseudo code representation.
- **Integrate** with [Gainliner.com](https://gainliner.com) to provide an in-browser code visualization experience.

With this converter, developers or learners can quickly see how C# syntax can be turned into more general, language-independent pseudo code. This is especially useful for:

- Teaching programming concepts.
- Sharing algorithmic logic across multiple language communities.
- Documenting or discussing code without tying it to a specific programming language.

---

## Current Status

**Early Stage / Quick First Attempt**

- This repository is in a **very early** development phase.
- The current implementation is a **minimal prototype** focusing on simple constructs in C#.
- **Disclaimer**: Expect incomplete features, rough edges, and plenty of bugs!

Key limitations right now:

- Only **partial support** for C# features (limited statement coverage).
- Rudimentary handling of **classes and methods**.
- Minimal error handling and testing.
- Not yet production-ready.

---

## How It Works

1. **Parse**: The tool reads C# code lines and identifies key elements (like method signatures, loops, etc.).
2. **Transform**: Each identified element is converted to a pseudo code equivalent (e.g., `for` loop → `for each iteration ...`).
3. **Output**: The transformed pseudo code is printed or saved, ready for further formatting or display on [Gainliner.com](https://gainliner.com).

```plaintext
C# code                                                 Pseudo code
--------------------------------------------------------------------
public void Hello()                                 function Hello():
{                                                       print "Hi"
    Console.WriteLine("Hi");
}

```
## Roadmap

- **Improved Parsing**:
Expand on the parser to handle more C# constructs (interfaces, generics, LINQ, etc.).

- **Error Handling**
Add robust handling for parsing errors or incomplete code snippets.

- **Modular Output**
Offer multiple pseudo code "flavors" (e.g., more verbose, or closer to pseudocode standards).

- **Integration**
Seamless integration with Gainliner.com for a streamlined workflow.

- **Testing & Examples**
Provide comprehensive test coverage and sample code for typical scenarios.
