import { singleton } from "tsyringe";
import { IClientMoneyService } from "./IClientMoneyService";
import { InsurerRating } from "@/entities/client-money/InsurerRating";

@singleton()
export default class ClientMoneyMockService implements IClientMoneyService {
  getInsurerRatingsAsync(_company: string): Promise<InsurerRating> {
    return Promise.resolve({ rating: "AA+", issuer: "S&P Ratings" });
  }
}
