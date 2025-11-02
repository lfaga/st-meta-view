# Safetensors Metadata Viewer for Windows (WPF)

![Application Icon](https://github.com/lfaga/st-meta-view/blob/main/.github/icon.png)

A desktop utility for Windows, built in C# and WPF, for inspecting the metadata of `.safetensors` model files. This tool was created to provide a clear, hierarchical, and interactive view of the embedded metadata that is often complex and hard to read in a standard text editor.

![Application Screenshot](https://github.com/lfaga/st-meta-view/blob/main/.github/screencap.png)

---

### The Origin Story

My journey into generative AI began with Python, where I wrote a simple command-line script to dump `.safetensors` metadata to a JSON file (available in my [AI Notebooks Portfolio](https://github.com/lfaga/Generative-AI-Notebook-Projects)).

Wanting a richer, more interactive graphical interface, I decided to leverage my deep background in .NET and build a native WPF desktop application. At the time, I couldn't find a C# library for reading the `.safetensors` format. I took this on as a challenge, which involved two key engineering problems:
1.  Writing a custom binary parser from scratch to correctly read the file header and extract the raw metadata payload.
2.  Building a robust deserializer to handle the often malformed, nested JSON strings contained within the metadata.

This project is the result: a practical tool born from a real-world need.

---

### Key Features

*   **Hierarchical Tree-View:** Displays complex, nested metadata in a clean, expandable, tree-like structure for easy navigation.
*   **Interactive UI:** The interface is fully interactive.
    *   Click a column header to sort the data.
    *   Click a key to copy the full key-value pair to the clipboard.
    *   Click a value to copy only that value.
    *   Click a dictionary key (a parent in the tree) to copy the entire nested structure.
*   **User-Friendly Design:** Includes a dark theme for comfortable viewing and font-size controls for accessibility.
*   **Context-Aware Feedback:** Tooltips guide the user, and a status bar provides confirmation of actions like copying data.

---

### Architectural Highlights

This project is a testament to first-principles engineering. Two components are of particular technical interest:

1.  **Recursive, Programmatic UI Generation:** Instead of using a standard `TreeView` control, the entire hierarchical display is built dynamically in code. The `PopulateTree` function recursively nests `Grid` controls to construct the UI. This low-level approach provided granular control over the layout and enabled the implementation of highly specific, context-aware user interactions (like the different click behaviors for keys, values, and headers).

2.  **Custom Binary Parser & Deserializer:** The core of the application is its ability to read the `.safetensors` format natively. This was achieved by writing a custom parser that reads the 8-byte header to determine the metadata length, extracts the raw UTF-8 payload, and then runs a series of sanitization routines to correct malformed JSON before deserializing it into a structured object model for display.

---

### Technology Stack & Project Status

*   **Framework:** C# with Windows Presentation Foundation (WPF) on the .NET 4.0 Framework.
*   **Development Environment:** Visual Studio 2010.
*   **Status:** This is a functional and polished personal project.

**A Note on the Technology:** This project was developed on an older machine, and as such, it uses an older version of the .NET Framework. The value of this project lies in its architectural patterns—a recursive UI generator, a custom binary parser, and a thoughtful user experience—which are timeless and independent of the framework version.
