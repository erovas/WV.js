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
| `State`               | Gets or sets windows state. -1 = None; 0 = Minimized; 1 = Normalized; 2 = Maximized. |
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

IRect

| Property       | Description                                                                                             |
|--------------------------|---------------------------------------------------------------------------------------------------------|
| `X`               | Window position in X. |
| `Y`               | Window position in Y. |
| `Width`              | Window width. |
| `Height`  | Window Height. |
| `MaxWidth`| Maximum window width. |
| `MaxHeight`  | Maximum window height. |
| `MinWidth`  | Minimun window width. |
| `MinHeight`  | Minimun window height. |

| Method       | Description                                                                                             |
|--------------------------|---------------------------------------------------------------------------------------------------------|
| `SetSize(width, height)`               | Set size of window. |
| `GetSize()`               | Get size of window. [number, number] == [width, height] |
| `SetPosition(x, y)`       | Set position of window. |
| `GetPosition()`  | Get position of window. [number, number] == [x, y] |
| `SetPositionAndSize(x, y, width, height)`| Set position and size of window |
| `GetPositionAndSize()`| Get position and size of window. [number, number, number, number] == [x, y, width, height] |


IBrowser

| Property       | Description                                                                                             |
|--------------------------|---------------------------------------------------------------------------------------------------------|
| `Uri`               | Gets the URI of the current top level document. |
| `HotReload`  | Enable or disable automatic browser reloading, use it during development. Default value is false. |
| `CanGoBack`    | True if the WebView is able to navigate to a previous page in the navigation history. |
| `CanGoForward`  | True if the WebView is able to navigate to a next page in the navigation history. |
| `IsPlayingAudio`| Indicates whether any audio output from this CoreWebView2 is playing. true if audio is playing even if IBrowser.Muted is true. |
| `AcceleratorKeys`  | Determines whether browser-specific accelerator keys are enabled. Default value is true. |
| `SwipeNavigation`  | Determines whether the end user to use swiping gesture on touch input enabled devices to navigate in WebView. Swiping gesture navigation on touch screen includes: Swipe left/right (swipe horizontally) to navigate to previous/next page in navigation history. Default value is false. |
| `ZoomFactor`  | Gets or sets the zoom factor for the WebView. |
| `MaxZoomFactor`  | Gets maximum ZommFactor. |
| `MinZoomFactor`  | Gets minimum ZoomFactor. |
| `ResetWebViewOnReload`  | When WebView is reload, reset all settings to default. Default value is false. |
| `ContextMenu`  | Gets IContextMenu instance. |
| `Muted`  | Indicates whether all audio output from this WebView2 is muted or not. Set to true will mute this CoreWebView2, and set to false will unmute this WebView2. True if audio is muted. |
| `StatusBarText`  | Get last status bar text. |
| `ColorScheme`  | Get or set browser color scheme. 0 = Auto; 1 = Light; 2 = Dark. |
| `ColorSchemeText`  | Gets or sets the browser's color scheme by text. |
| `AddEventListener(type, callback)`| Appends an event listener for events whose type attribute value is type. |
| `RemoveEventListener(type, callback)`| Removes the event listener in target's event listener list with the same type and callback. |


| Method       | Description                                                                                             |
|--------------------------|---------------------------------------------------------------------------------------------------------|
| `OpenDevTools()`   | Open developer tools [DevTools]. |
| `Navigate(uri)`    | Navigate to a specific URI. |
| `Reload()`         | Reload the page. |
| `HardReload()`     | Reload the page avoiding the cache. |
| `ExecuteScriptAsync(javaScript)` | Runs JavaScript code from the javaScript parameter in the current WebView. |
| `GoBack()`| Navigates the WebView to the previous page in the navigation history. |
| `GoForward()`| Navigates the WebView to the next page in the navigation history. |


| Event       | Description                                                                                             |
|--------------------------|---------------------------------------------------------------------------------------------------------|
| `PlayingAudio, OnPlayingAudio`  | PlayingAudio Event. It is triggered when the browser is playing audio. (boolean) == (isPlayingAudio) |
| `MutedEvent, OnMuted`  | Muted Event. It is triggered when the browser is muted or unmute. (boolean) == (muted) |
| `ZoomFactorChanged, OnZoomFactorChanged` | ZoomFactor changed event (number) == (zoomFactor)|
| `StatusBarTextChanged, OnStatusBarTextChanged`| StatusBarTextChanged event. (string) == (statusBarText)|


IContextMenu

| Property       | Description                                                                                             |
|--------------------------|---------------------------------------------------------------------------------------------------------|
| `Enable`               | Enable or disable context menu. |
| `Children`               | Gets all cotext menu items. [...IContextMenuItem] |
| `ShowNativeItems`      | Enable or disable native items. |
| `EmojiItem, UndoItem, RedoItem, CutItem, CopyItem, PasteItem, PasteAndMatchStyleItem, SelectAllItem, WritingDirectionItem, ShareItem, WebCaptureItem, LoopItem, ShowAllControlsItem, SaveMediaAsItem, CopyLinkItem, CopyLinkToHighlightItem, PrintItem, BackItem, ForwardItem, ReloadItem, SaveAsItem, SaveImageAsItem, CopyImageItem, CopyImageLocationItem, MagnifyImageItem, SaveFrameAsItem, CopyVideoFrameItem, PictureInPictureItem, SaveLinkAsItem, OpenLinkInNewWindowItem`  | Enable or disable individual native item. |

| Method       | Description                                                                                             |
|--------------------------|---------------------------------------------------------------------------------------------------------|
| `CreateContextItem(name, kind, icon = null, callback = null)`               | Create an IContextMenuItem instance. name = text to show; Kind = "Command", "CheckBox", "Radio", "Separator", "Submenu"; icon = Absolute or relative path to icon; callback = function(kind, checked)  |
| `CreateContextItemSeparator()`  | Create an IContextMenuItem instance as separator. |
| `AddItem(item)`       | Add the item at the end. |
| `RemoveItem(item)`  | Removes the item. Return true if removed. |
| `RemoveItemAt(index)`| Removes the item to the specified position. |
| `Clear()`| Remove all items. |
