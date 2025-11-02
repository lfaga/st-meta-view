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

This project, while small, is a case study in pragmatic, first-principles engineering to solve real-world data challenges.

+ **Custom Binary Parser:** Lacking a C# library for the .safetensors format, the application's core is a custom parser written from scratch. It reads the 8-byte file header to determine the metadata length and then extracts the raw UTF-8 JSON payload for processing.

+ **Recursive JSON Expansion Engine:** The metadata in .safetensors files is often a complex, multi-level structure where nested JSON objects and arrays are themselves encoded as strings. To handle this, a custom, recursive parsing engine was developed. It uses System.Text.Json's JsonDocument model to:

    - Parse the initial JSON payload into a JsonElement tree.

    - Intelligently walk the tree, identifying JsonElements of type String that are themselves valid JSON documents.

    - Recursively parse these nested documents, correctly handling both objects and arrays found within strings.

    - The final result is a clean, fully-hydrated, and native Dictionary<string, object> that represents the complete, deeply-nested structure of the metadata.

+ **Programmatic UI Generation:** To achieve the specific, multi-column hierarchical layout required, the entire data grid is built dynamically in C# code-behind. A recursive PopulateTree function programmatically creates and nests Grid controls, providing the granular, low-level control necessary to implement the context-aware sorting and click-to-copy interactions.

---

### Technology Stack & Project Status

*   **Framework:** C# with Windows Presentation Foundation (WPF) on .NET 8.0.
*   **Development Environment:** Visual Studio 2022.
*   **Status:** This is a functional and polished personal project.

