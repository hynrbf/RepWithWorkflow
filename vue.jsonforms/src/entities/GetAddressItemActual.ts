export class GetAddressItemActual {
    public postcode: string | undefined;
    public latitude: number = 0;
    public longitude: number = 0;
    public formatted_address: string[] = [];
    public thoroughfare: string | undefined;
    public building_name: string | undefined;
    public sub_building_name: string | undefined;
    public sub_building_number: string | undefined;
    public building_number: string | undefined;
    public line_1: string | undefined;
    public line_2: string | undefined;
    public line_3: string | undefined;
    public line_4: string | undefined;
    public locality: string | undefined;
    public town_or_city: string | undefined;
    public county: string | undefined;
    public district: string | undefined;
    public country: string | undefined;
    public residential: boolean = false;
}