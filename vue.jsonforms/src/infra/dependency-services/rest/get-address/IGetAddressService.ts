import {GetAddressItem} from "@/entities/GetAddressItem";
import {GetAddressItemActual} from "@/entities/GetAddressItemActual";

export declare interface IGetAddressService {
    getFilteredAddressAsync(keyword: string): Promise<GetAddressItem[]>;
    getActualAddressAsync(id: string): Promise<GetAddressItemActual>;
}

export const IGetAddressServiceInfo = {
    name: "IGetAddressService"
};