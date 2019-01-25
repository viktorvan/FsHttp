
namespace FsHttp

[<AutoOpen>]
module Domain =

    open System
    open System.Net.Http

    type Header = {
        url: string;
        method: HttpMethod;
        headers: (string*string) list;
    }

    type Content = {
        content: string;
        contentType: string;
        headers: (string*string) list;
    } 

    type StartingContext = StartingContext

    type FinalContext = {
        request: Header;
        content: Content option;
    }

    type HeaderContext = { request: Header } with
        static member Header (this: HeaderContext, name: string, value: string) =
            { this with request = { this.request with headers = this.request.headers @ [name,value] } }
        static member Finalize (this: HeaderContext) =
            let finalContext = { request=this.request; content=None }
            finalContext

    type BodyContext = { request: Header; content: Content; } with
        static member Header (this: BodyContext, name: string, value: string) =
            { this with request = { this.request with headers = this.request.headers @ [name,value] } }
        static member Finalize (this: BodyContext) =
            let finalContext:FinalContext = { request=this.request; content=Some this.content }
            finalContext

    type Response = {
        requestContext: FinalContext;
        requestMessage: HttpRequestMessage;
        content: HttpContent;
        headers: Headers.HttpResponseHeaders;
        reasonPhrase: string;
        statusCode: System.Net.HttpStatusCode;
        version: Version;
        printHint: PrintHint
    }
    and PrintHint = {
        requestPrintHint: RequestPrintHint;
        responsePrintHint: ResponsePrintHint;
    }
    and RequestPrintHint = {
        enabled: bool;
        printHeader: bool
    }
    and ResponsePrintHint = {
        enabled: bool;
        printHeader: bool;
        printContent: ContentPrintHint
    }
    and ContentPrintHint = {
        enabled: bool;
        format: bool;
        maxLength: int
    }

    type HttpBuilder() = class end
