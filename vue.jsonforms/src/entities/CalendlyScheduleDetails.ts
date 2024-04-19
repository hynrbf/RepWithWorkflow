export interface CalendlyScheduleCollection {
    collection: CalendlyScheduleDetails[];
}

export interface CalendlyScheduleDetails {
    name: string;
    active: boolean;
    scheduling_url: string
}