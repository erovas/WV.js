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
| `Window`              | Return IWindow instance |
| `Browser`  | Return IBrowser instance |
| `PrintManager`| Return IPrintManager instance |

| Method       | Description                                                                                             |
|--------------------------|---------------------------------------------------------------------------------------------------------|
| `LoadPluginsFromFolder(folderPath = "")`               | If the parameter is not set, all plugins will be loaded from the plugins folder, otherwise the parameter must be an absolute path. Returns an array of strings with the plugins that have failed to load.  |
| `LoadPlugin(pluginPath)`               | Load plugin from path. Parameter can be relative or absolute path. Return plugin name. |
| `UnloadPlugin(pluginName)`              | Unload plugin by name |
| `NewPluginInstance(pluginName, params args)`  | Create an instance of a plugin. An instance of a plugin contains a unique UID |
| `GetPluginInstance(UID)`| Retrieves a plugin instance using its UID |
| `Restart()`| Restart the application if WebView is the main one, otherwise throw exception |


IWindow
