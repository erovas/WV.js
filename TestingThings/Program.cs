// See https://aka.ms/new-console-template for more information
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text.Json;
using TestingThings;

Console.WriteLine("Hello, World!");

Uri uriResult;
bool result = Uri.TryCreate("https://aka.ms/new-console-template", UriKind.RelativeOrAbsolute, out uriResult);
//&& uriResult.Scheme == Uri.UriSchemeHttp;
result = Uri.TryCreate("/new-console-template", UriKind.RelativeOrAbsolute, out uriResult);
/*
result = Uri.TryCreate("/new-console-template", UriKind.Relative, out uriResult);
result = Uri.TryCreate("//new-console-template", UriKind.RelativeOrAbsolute, out uriResult);
result = Uri.TryCreate("/////new-console-template/", UriKind.RelativeOrAbsolute, out uriResult);
result = Uri.TryCreate("new-console-template", UriKind.RelativeOrAbsolute, out uriResult);
*/
Console.WriteLine("Es " + result);

// ASi busco la clase que haya implementado la interfaz, para poderla utilizar para
// deserializar el json y convertirlo a la interfaz
var type = typeof(IAlgo);
var types = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(s => s.GetTypes())
    .Where(p => type.IsAssignableFrom(p) && p != type).ToList();

/*IAlgo? alguito = (IAlgo)JsonSerializer.Deserialize(
    @"{ 
        ""Name"": ""Nombre"", 
        ""Description"": ""Descripción"",
        ""tipo"": ""RelativeOrAbsolute""
    }", types[0]);
*/

IAlgo? alguito = (IAlgo)JsonSerializer.Deserialize(
    @"{ }", types[0]);


ITipo tipo = new ImTipo();

IGenerico<string> tipoGenerico = (IGenerico<string>)tipo;


ImClaseAbstracta DSA = new ImClaseAbstracta();

ClaseAbstracta DSAAA = DSA;

Console.WriteLine("Valor " + DSAAA.Value);
//List<string> keys = new List<string>();
//keys.Add("asasa");
//keys.Add("asasa");
//keys.Add("asasa");
//var inmutableasd = ImmutableList.Create;

Dictionary<string, object> data = new Dictionary<string, object>();
data.Add("buleano", true);
data.Add("numero", 12345);
data.Add("string", "ola ke ase");

string jsonString = JsonSerializer.Serialize(data);

Uri uri1 = new Uri("C:/test/path/file.txt"); // Implicit file path.
Uri uri2 = new Uri("file:///C:/test/path/file.txt"); // Explicit file path.
Uri uri3 = new Uri("C:/test/path/"); // Implicit file path.


//Notify<List<string>> prueba = new Notify<List<string>>("hola");

var prueba = new Notify<Exception>();

prueba = new Notify<Exception>(new Exception("aswdasdasd"));

var prueba2 = new Notify<int>();

Console.ReadLine();