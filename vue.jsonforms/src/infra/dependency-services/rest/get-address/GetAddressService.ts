import { GetAddressItem } from "@/entities/GetAddressItem";
import { IGetAddressService } from "./IGetAddressService";
import { REMOTE_API, USE_REMOTE_DB } from "@/config";
import RestBase from "../RestBase";
import { GetAddressItemActual } from "@/entities/GetAddressItemActual";
import { singleton } from "tsyringe";
import { GetAddressItemWithSuggestions } from "@/entities/firm-details/GetAddressItemWithSuggestions";

@singleton()
export default class GetAddressService
  extends RestBase
  implements IGetAddressService
{
  async getFilteredAddressAsync(keyword: string): Promise<GetAddressItem[]> {
    if (!USE_REMOTE_DB) {
      throw new Error(
        "Searching addresses will not work if run locally. Please connect to outside services.",
      );
    }

    if (!keyword) {
      return [] as GetAddressItem[];
    }

    const capitalizedKeyword = keyword.toUpperCase();
    const endpoint = `${REMOTE_API}/PostGetFilteredAddressAsync`;
    const payload = {
      keyword: capitalizedKeyword,
      countryFilter: "",
    };

    const response = await this.postRemoteAsync<GetAddressItemWithSuggestions>(
      endpoint,
      JSON.stringify(payload),
    );
    return response.suggestions;
  }

  async getActualAddressAsync(id: string): Promise<GetAddressItemActual> {
    if (!USE_REMOTE_DB) {
      throw new Error(
        "Getting actual address will not work if run locally. Please connect to outside services.",
      );
    }

    if (!id) {
      return new GetAddressItemActual();
    }

    return await this.getRemoteAsync<GetAddressItemActual>(
      `${REMOTE_API}/GetActualAddressAsync/${id}`,
    );
  }
}
