const NAME = 'FileSystem';
const WV = window.wv;
const WV_ASYNC = WV.async();
const New = WV.new;
const NewAsync = WV_ASYNC.new;

WV.newFileSystem = NewFileSystem;
WV_ASYNC.newFileSystem = NewFileSystemAsync;

WV.new = (name, ...args) => {
    if (name === NAME)
        return NewFileSystem();
    
    return New(name, ...args);
}

WV_ASYNC.new = async (name, ...args) => {
    if (name === NAME)
        return await NewFileSystemAsync();

    return await NewAsync(name, ...args);
}

function NewFileSystem() {
    return New(NAME);
}

async function NewFileSystemAsync() {
    return await NewAsync(NAME);
}