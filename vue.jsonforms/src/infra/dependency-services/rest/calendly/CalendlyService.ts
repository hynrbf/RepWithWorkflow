import RestBase from "../RestBase";
import { ICalendlyService } from "./ICalendlyService";
import { REMOTE_API, USE_REMOTE_DB } from "@/config";
import { singleton } from "tsyringe";
import {
  CalendlyScheduleCollection,
  CalendlyScheduleDetails,
} from "@/entities/CalendlyScheduleDetails";

@singleton()
export default class CalendlyService
  extends RestBase
  implements ICalendlyService
{
  async getSchedulingLinkAsync(): Promise<CalendlyScheduleDetails[]> {
    if (!USE_REMOTE_DB) {
      throw new Error(
        "CalendlyService.getSchedulingLinkAsync: Getting scheduling link will not work if run locally. Please connect to outside services.",
      );
    }

    const collectionResult =
      await this.getRemoteAsync<CalendlyScheduleCollection>(
        `${REMOTE_API}/GetCalendlyEventTypesAsync`,
      );

    return collectionResult?.collection ?? [];
  }
}
