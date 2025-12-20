# WV.js
WV.js is a framework to build desktop applications with simple html, css and js, without node.js dependency.

## Compatibility

For Windows 10 or higher.

## How to use?

``` html
<script src="script.js"></script>
```

### API
IWebView

| Property       | Description                                                                                             |
|--------------------------|---------------------------------------------------------------------------------------------------------|
| `UID`               | Gets a value that identify this WebView |
| `Name`               | Plugin name |
| `Window`              | Gets a IWindow instance |
| `Browser`  | Gets a IBrowser instance |
| `PrintManager`| Return IPrintManager instance |

| Method       | Description                                                                                             |
|--------------------------|---------------------------------------------------------------------------------------------------------|
| `LoadPluginsFromFolder(folderPath = "")`               | If the parameter is not set, all plugins will be loaded from the plugins folder, otherwise the parameter must be an absolute path. Returns an array of strings with the plugins that have failed to load.  |
| `LoadPlugin(pluginPath)`               | Load plugin from path. Parameter can be relative or absolute path. Return plugin name. |
| `UnloadPlugin(pluginName)`              | Unload plugin by name. |
| `NewPluginInstance(pluginName, ...args)`  | Create an instance of a plugin. An instance of a plugin contains a unique UID. |
| `GetPluginInstance(UID)`| Retrieves a plugin instance using its UID. |
| `Restart()`| Restart the application if WebView is the main one, otherwise throw exception. |


IWindow

| Property       | Description                                                                                             |
|--------------------------|---------------------------------------------------------------------------------------------------------|
| `Rect`               | Gets a IRect instance. Which has position and size of the window. |
| `State`               | Gets or sets windows state. -1 = None; 0 = Minimized; 1 = Normalized; 2 = Maximized|
| `StateText`              | Gets or sets windows state by text. |
| `Title`  | Gets or sets a WebView's title. |
| `TopMost`| Gets or sets a value that indicates whether this WebView appears in the topmost z-order. True if the window is topmost; otherwise, false. |
| `Enabled`  | Gets or sets a value indicating whether this WebView is enabled in the user interface (UI). True if the WebView is enabled; otherwise, false. The default value is true. |
| `IsVisible`  | Gets a value that indicates whether this WebView window is visible. True if the WebView is visible; otherwise, false. The default value is false. |
| `PreventClose`  | Prevents the window from closing, and fires the OnClose event. |
| `IsActive`  | Gets a value that indicates whether this WebView window is active. |
| `AllowSnap`  | Get or set the snap window function. Default is true. |
| `ClickThrough`  | Get or set ClickThrough. |

| Method       | Description                                                                                             |
|--------------------------|---------------------------------------------------------------------------------------------------------|
| `ToCenter()`               | Moves the window to the center of the screen. |
| `Close()`               | Close the window. If PreventClose is true, OnClose event is fire. |
| `ShowBehind()`              | Shows the window without activating it. |
| `Show()`  | Shows the window. |
| `Hide()`| Hide the window. |
| `Drag()`| Drag the window. |
| `ResizeTopLeft()`| Resize top-left the window. |
| `ResizeTopRight()`| Resize top-right the window. |
| `ResizeBottomLeft()`| Resize bottom-left the window. |
| `ResizeBottomRight()`| Resize bottom-right the window. |
| `ResizeLeft()`| Resize left the window. |
| `ResizeRight()`| Resize right the window. |
| `ResizeTop()`| Resize top the window. |
| `ResizeBottom()`| Resize bottom the window. |
| `Minimize()`| Minimize the window. |
| `Restore()`| Restore the window. |
| `Maximize()`| Maximize the window. |
| `Normalize()`| Normalize the window. |
| `AddEventListener(type, callback)`| Appends an event listener for events whose type attribute value is type. |
| `RemoveEventListener(type, callback)`| Removes the event listener in target's event listener list with the same type and callback. |

| Event       | Description                                                                                             |
|--------------------------|---------------------------------------------------------------------------------------------------------|
| `StateChanged, OnStateChanged`               | StateChanged event. Fire when WindowState change. (integer, string) == (State, StateText) |
| `Closing, OnClosing`               | Close event. It is triggered when the PreventClose property is true and try to close the window. |
| `PositionChanged, OnPositionChanged`              | PositionChanged event. It is triggered when the window changes position. (number, number) == (X, Y)|
| `Activated, OnActivated`| Activated event. (boolean) == (activated)|
| `EnabledEvent, OnEnabled`| Enabled event. (boolean) == (enabled)|
| `Visible, OnVisible`| Visible event. (boolean) == (visible)|
| `SizeChanged, OnSizeChanged`| Size Changed event. (number, number) == (width, height)|
| `Raw`| Raw event for platform specific. To use it in plugins C#. |

