import {CalendlyScheduleDetails} from "@/entities/CalendlyScheduleDetails";

export declare interface ICalendlyService {
    getSchedulingLinkAsync(): Promise<CalendlyScheduleDetails[]>;
}

export const ICalendlyServiceInfo = {
    name: "ICalendlyService"
};