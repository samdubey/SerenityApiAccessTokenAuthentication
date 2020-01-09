namespace SereneApp.Northwind {
    export namespace CustomerApiService {
        export const baseUrl = '~/Api/Services/Northwind/Customer';

        export declare function Create(request: Serenity.SaveRequest<CustomerRow>, onSuccess?: (response: Serenity.SaveResponse) => void, opt?: Q.ServiceOptions<any>): JQueryXHR;
        export declare function Update(request: Serenity.SaveRequest<CustomerRow>, onSuccess?: (response: Serenity.SaveResponse) => void, opt?: Q.ServiceOptions<any>): JQueryXHR;
        export declare function Delete(request: Serenity.DeleteRequest, onSuccess?: (response: Serenity.DeleteResponse) => void, opt?: Q.ServiceOptions<any>): JQueryXHR;
        export declare function GetNextNumber(request: GetNextNumberRequest, onSuccess?: (response: GetNextNumberResponse) => void, opt?: Q.ServiceOptions<any>): JQueryXHR;
        export declare function Retrieve(request: Serenity.RetrieveRequest, onSuccess?: (response: Serenity.RetrieveResponse<CustomerRow>) => void, opt?: Q.ServiceOptions<any>): JQueryXHR;
        export declare function List(request: Serenity.ListRequest, onSuccess?: (response: Serenity.ListResponse<CustomerRow>) => void, opt?: Q.ServiceOptions<any>): JQueryXHR;

        export declare const enum Methods {
            Create = "~/Api/Services/Northwind/Customer/Create",
            Update = "~/Api/Services/Northwind/Customer/Update",
            Delete = "~/Api/Services/Northwind/Customer/Delete",
            GetNextNumber = "~/Api/Services/Northwind/Customer/GetNextNumber",
            Retrieve = "~/Api/Services/Northwind/Customer/Retrieve",
            List = "~/Api/Services/Northwind/Customer/List"
        }

        [
            'Create', 
            'Update', 
            'Delete', 
            'GetNextNumber', 
            'Retrieve', 
            'List'
        ].forEach(x => {
            (<any>CustomerApiService)[x] = function (r, s, o) {
                return Q.serviceRequest(baseUrl + '/' + x, r, s, o);
            };
        });
    }
}
