const NAME = 'SQLite';
const WV = window.wv;
const WV_ASYNC = WV.async();
const New = WV.new;
const NewAsync = WV_ASYNC.new;

WV.newSQLite = NewSQLite;
WV_ASYNC.newSQLite = NewSQLiteAsync;

WV.new = (name, ...args) => {
    if (name === NAME)
        return NewSQLite(...args);

    return New(name, ...args);
}

WV_ASYNC.new = async (name, ...args) => {
    if (name === NAME)
        return await NewSQLiteAsync(...args);

    return await NewAsync(name, ...args);
}

function NewSQLite(connectionString) {
    if(connectionString === undefined || connectionString === null)
        return New(NAME);

    return New(NAME, connectionString + '');
}

async function NewSQLiteAsync(connectionString) {
    if (connectionString === undefined || connectionString === null)
        return await NewAsync(NAME);

    return await NewAsync(NAME, connectionString + '');
}