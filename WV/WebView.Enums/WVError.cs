using System.Text.Json.Serialization;

namespace WV.WebView.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WVError
    {
        /// <summary>
        /// Indicates that the browser process ended unexpectedly
        /// </summary>
        BrowserProcessExited,

        /// <summary>
        /// Indicates that the main frame's render process ended unexpectedly
        /// </summary>
        RenderProcessExited,

        /// <summary>
        /// Indicates that the main frame's render process is unresponsive.
        /// </summary>
        RenderProcessUnresponsive,

        /// <summary>
        /// Indicates that a frame-only render process ended unexpectedly.
        /// </summary>
        FrameRenderProcessExited,

        /// <summary>
        /// Indicates that a utility process ended unexpectedly.
        /// </summary>
        UtilityProcessExited,

        /// <summary>
        /// Indicates that a sandbox helper process ended unexpectedly.
        /// </summary>
        SandboxHelperProcessExited,

        /// <summary>
        /// Indicates that the GPU process ended unexpectedly.
        /// </summary>
        GpuProcessExited,

        /// <summary>
        /// Indicates that a PPAPI plugin process ended unexpectedly.
        /// </summary>
        PpapiPluginProcessExited,

        /// <summary>
        /// Indicates that a PPAPI plugin broker process ended unexpectedly.
        /// </summary>
        PpapiBrokerProcessExited,

        /// <summary>
        /// Indicates that a process of unspecified kind ended unexpectedly.
        /// </summary>
        UnknownProcessExited,

        /// <summary>
        /// Indicates that an unknown error occurred.
        /// </summary>
        Unknown,

        /// <summary>
        /// Indicates that the SSL certificate common name does not match the web address.
        /// </summary>
        CertificateCommonNameIsIncorrect,

        /// <summary>
        /// Indicates that the SSL certificate has expired.
        /// </summary>
        CertificateExpired,

        /// <summary>
        /// Indicates that the SSL client certificate contains errors.
        /// </summary>
        ClientCertificateContainsErrors,

        /// <summary>
        /// Indicates that the SSL certificate has been revoked.
        /// </summary>
        CertificateRevoked,

        /// <summary>
        /// Indicates that the SSL certificate is not valid. The certificate may not match
        /// the public key pins for the host name, the certificate is signed by an untrusted
        /// authority or using a weak sign algorithm, the certificate claimed DNS names violate
        /// name constraints, the certificate contains a weak key, the validity period of
        /// the certificate is too long, lack of revocation information or revocation mechanism,
        /// non-unique host name, lack of certificate transparency information, or the certificate
        /// is chained to a [legacy Symantec root](https://security.googleblog.com/2018/03/distrust-of-symantec-pki-immediate.html).
        /// </summary>
        CertificateIsInvalid,

        /// <summary>
        /// Indicates that the host is unreachable.
        /// </summary>
        ServerUnreachable,

        /// <summary>
        /// Indicates that the connection has timed out.
        /// </summary>
        Timeout,

        /// <summary>
        /// Indicates that the server returned an invalid or unrecognized response.
        /// </summary>
        ErrorHttpInvalidServerResponse,

        /// <summary>
        /// Indicates that the connection was stopped.
        /// </summary>
        ConnectionAborted,

        /// <summary>
        /// Indicates that the connection was reset.
        /// </summary>
        ConnectionReset,

        /// <summary>
        /// Indicates that the Internet connection has been lost.
        /// </summary>
        Disconnected,

        /// <summary>
        /// Indicates that a connection to the destination was not established.
        /// </summary>
        CannotConnect,

        /// <summary>
        /// Indicates that the provided host name was not able to be resolved.
        /// </summary>
        HostNameNotResolved,

        /// <summary>
        /// Indicates that the operation was canceled. This status code is also used in the following cases:
        /// </summary>
        OperationCanceled,

        /// <summary>
        /// Indicates that the request redirect failed.
        /// </summary>
        RedirectFailed,

        /// <summary>
        /// An unexpected error occurred.
        /// </summary>
        UnexpectedError,

        /// <summary>
        /// Indicates that user is prompted with a login, waiting on user action. Initial
        /// navigation to a login site will always return this even if app provides credential
        /// using Microsoft.Web.WebView2.Core.CoreWebView2.BasicAuthenticationRequested.
        /// HTTP response status code in this case is 401. See status code reference here:
        /// https://developer.mozilla.org/docs/Web/HTTP/Status.
        /// </summary>
        ValidAuthenticationCredentialsRequired,

        /// <summary>
        /// Indicates that user lacks proper authentication credentials for a proxy server.
        /// HTTP response status code in this case is 407. See status code reference here:
        /// https://developer.mozilla.org/docs/Web/HTTP/Status.
        /// </summary>
        ValidProxyAuthenticationRequired
    }
}