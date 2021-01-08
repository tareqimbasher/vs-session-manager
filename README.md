# Session Manager for Visual Studio
Easily save and restore document sessions. Supports Visual Studio 2019.

## Use
---
Its always annoying when you switch context to work on something else. Its tough enough getting back your thought train, restoring your workspace to how it was before you switched is a great first step. **Session Manager** helps you save a session of all the opened documents, give it a contextual name, and then be able to restore those opened documents any time you'd like.

## How to use **Session Manager**
---
**Session Manager** adds a new tool window pane to Visual Studio: **View > Other Windows > Session Manager**

![Saved Tabs Window](src/SessionManagerExtension/Resources/SessionManager-Preview.png?raw=true "Saved Tabs Window")

### Commands
* **Save Current Session** - Saves a new session that captures the documents you currently have opened.
* **Restore Session** - Restores the selected session, closing any documents that are currently opened.
* **Open** - Opens the documents saved in the selected session without closing any opened documents. You can also double-click a session to open it.
* **Delete Session** - Deletes the selected session.

### Context Menu
Right-click any of the sessions to access these additional commands:
* **Rename** - Rename an existing session.
* **Save opened documents to this session** - Updates the selected session with the documents you currently have opened, replacing the documents already saved in that session.