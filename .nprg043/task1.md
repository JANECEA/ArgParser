# Task #1: API Design

The assignments deal with the design and implementation of a library for
parsing command-line arguments. It is a well-defined problem which leaves a
lot of room for different design decisions, and allows different solutions
that can all be considered good.

Before presenting the requirements, let's start with some basic terminology.

## Terminology

Short option
: A command-line argument that starts with a single dash,
  followed by a single character (e.g. "`-v`")

Long option
: A command-line argument that starts with two dashes,
  followed by one or more characters (e.g. "`--version`")

Option
: Short option or Long option

Option parameter
: A command-line argument that follows an option (if the option is defined
  to accept parameters by the user of your library)

Plain argument
: A command-line argument that is neither an option nor an option parameter.

Delimiter
: A command-line argument consisting of two dashes only, i.e. `--`.
  Any subsequent argument is considered to be a plain argument

For example, the following command

```shell
cmd -v --version -s OLD --length=20 -- -my-file your-file
```

contains short options `v` and `s`, long options `version` and `length`, the
`OLD` and `20` values represent arguments to `-s` and `--length` options,
respectively. The `-my-file` and `your-file` arguments after the `--`
delimiter are plain arguments.

## API requirements

This is the first task in a sequence of tasks, and you are required to
**design an API** that allows the user to perform the following operations in
a convenient way:

- Specify options and arguments that a program accepts, specify which of
  them are optional or mandatory, and verify that the actual arguments passed
  to the program conform to the command line specification.
- Define option aliases (at the very least 1:1 between short and long
  options, but ideally in a more general way).
- Specify whether an option may/may not/must accept parameters (and how many)
  and verify that the actual arguments passed to the client program conform to
  the specification.
- Specify types of option parameters and plain arguments and verify that the
  actual arguments passed to the client program conform to the specification.
  At the very least the library has to distinguish between string parameters,
  integral parameters (with either or both: lower and upper bound), boolean
  parameters, and string parameters with fixed domain (i.e., enumeration).
- Document plain arguments and options and present the documentation to the
  user in form of a help text.
- Access the values of options and all plain arguments so that the client
  program can make runtime decisions based on the command line inputs.
- Allow mixing of plain arguments and options on the commandline (unless a
  plain argument starts with `-`, in which case it must come after the `--`
  delimiter).

At this stage, you are not supposed to implement the API &mdash; you will be
doing that later in another task. For now, write the interface parts of the
API (types, classes, methods with signatures), but leave the implementations
**EMPTY** &mdash; except for statements such as `return null;` or other
language-specific statements need for empty functions to compile.

It is not necessary to support all the above requirements explicitly. For
example, the validation of a type of a parameter may be performed implicitly
upon retrieval of the value by the user. An exception may be thrown if the
corresponding argument has an incorrect type.

The requirements are intentionally not exhaustive. Use your imagination and be
creative in the design...

### Design considerations

- How will the (library) user declare individual options, their
  parameters and aliases? What data structures or classes will represent them?
- How will the (library) user's program gain access to the parsed
  values? Callbacks? List of all options? On-demand access to
  particular options?
- In what way will the library validate the arguments?
  Explicitly/implicitly? How will the library report validation errors
  to the program user?
- What kind of errors can occur when using the library? How can the program
  using the library find out that an error has occurred (exceptions/error
  codes) and what can the program do about them?
- What classes will the library contain? What purpose will they have?

Don't forget to consider common corner cases:

- How will your API handle multiple occurrences of the same option?
- What happens if an option parameter closely resembles a short option
  (e.g., passing `-v` as the value for `-s`)?


### General suggestions

- Take a look at the existing libraries out there.
- Try to remedy their drawbacks and whatever annoyances you fancy.
- Modify some of your previous projects to use your API to parse its
  command-line options. This may help you discover design issues early
  in the process.

## Example program

Apart from the bare API, you must also submit an example program that uses
the API to parse the following arguments (obviously, it will fail, because the
library will lack the implementation) that are used by `time` command.

```text
time [options] command [arguments...]

GNU Options
    -f FORMAT, --format=FORMAT
           Specify output format, possibly overriding the format specified
           in the environment variable TIME.
    -p, --portability
           Use the portable output format.
    -o FILE, --output=FILE
           Do not send the results to stderr, but overwrite the specified file.
    -a, --append
           (Used together with -o.) Do not overwrite but append.
    -v, --verbose
           Give very verbose output about all the program knows about.

GNU Standard Options
    --help Print a usage message on standard output and exit successfully.
    -V, --version
           Print version information on standard output, then exit successfully.
    --     Terminate option list.
```

**DO NOT** implement the actual command, but **DO** implement mock output using
the parsed values. This will showcase both how to use your API to set up option
parsing for this command and how to access the parsed values.

## Submission

You will be given access to a Git repository hosted on the faculty GitLab
instance. You are required to commit your solution there. To make solutions
easier to handle (and to navigate by other students), each solution should
comprise the following components.

### Source code & Repository Structure

The main part of the solution is the source code of the library and the
example program. These are **two separate sub-projects** sharing the
same (multi-project) repository.

The source code (`.java`, `.cs`, `.cpp`, `.h`, etc.) **MUST** be cleanly
separated from build configuration files. Do not intermingle build scripts
(like `.vcxproj` or `pom.xml`) directly with your source files.

For example, an acceptable directory structure for a C++ project may look
as follows:

```text
/                           (repository root)
├── build/                  (Ignored by git; contains compiled binaries)
├── your-library/           (Project 1: The Library)
│   ├── src/                (Source code ONLY, possibly in subdirectories)
│   ├── include/            (Public headers, if using C/C++)
│   └── [build-file]        (e.g., CMakeLists.txt, build.gradle, .csproj)
├── example-time/           (Project 2: The Example Program)
│   ├── src/                (Source code ONLY, possibly in subdirectories)
│   └── [build-file]        (e.g., CMakeLists.txt, build.gradle, .csproj)
├── [global-build-file]     (e.g., .sln, parent pom.xml, root CMakeLists.txt)
├── README.md
└── .gitignore
```

The conventions for other languages will vary, but they generally adhere
to the principle of keeping files with distinct purposes separate.

The source code **MUST** compile.

### Documentation

You should document the library so that potential users can make sense of it.

A `README.md` with a basic description of the library, key concepts and use
cases, a simple and complex example, and instructions for building (including
dependencies) is an absolute minimum.

You **MUST** document all API elements using the standard documentation
format for your chosen language (e.g., Javadoc, XML Documentation Comments for
C#, Doxygen for C++). Treat these comments as contracts—describing legal
parameter values, nominal behavior, and error behavior. Without implementation,
the documentation may easily make up the most of your source code.

Generating the documentation from the sources is not required at this stage,
but having a build action for that purpose is welcome.

The landing page of the generated documentation is a good place to elaborate
your design decisions and the resulting key concepts and classes. This content
is not suitable for the README because it is too detailed, but it should be
accessible to someone who wants to understand your design. Also, writing about
the design may help you drive your design process. Creating a separate
`DESIGN.md` file is also fine at this point, especially if your project cannot
(yet) generate the documentation.

### Build/run instructions

Both the library and the example program projects must build and execute on a
Linux system and outside an IDE (as mentioned earlier, the example program is
expected to crash or throw an exception due to the missing implementation).

The `README.md` should provide instructions for building the projects and
running example programs. Ideally, each action should be a single command using
a standard build tool:

- **Java:** Maven or Gradle.
- **C++:** CMake or Make.
- **C#:** `dotnet` CLI (ensure the project runs on .NET Core/.NET 5+ on Linux).

Do **NOT** rely on IDE-specific "Run" buttons. You must verify that your build
instructions work in a terminal environment.

### `.gitignore`

Your repository must include a properly configured `.gitignore` file so that
you avoid committing:

- **Build Artifacts:** compiled files (`.class`, `.o`, `.obj`, `.exe`,
  `.dll`, `build/` directories).
- **IDE Metadata:** (hidden) IDE configuration folders (e.g., `.vs/`, `.idea/`,
  `.vscode/`) or OS-generated files (e.g., `.DS_Store`).

Repositories cluttered with these files will be penalized.

Also note that each repository will contain a `.nprg043` folder with files
associated with task management. Do **NOT** make this folder or its content
ignored.

### Submission tag

When you are satisfied with your solution to this task, make sure it is in the
**master** branch of your repository and **tag the commit** using the
`task-1-submission` tag (keep in mind that a tag is not a commit message). This
will indicate the state of the repository that should be evaluated.
