import { defineStore, storeToRefs } from "pinia";
import isEmpty from "lodash/isEmpty";
import { DocumentFormatting } from "@/entities/stationery/DocumentFormatting";
import { useCustomerStore } from "@/stores/useCustomerStore";
import StaticList from "@/infra/StaticListService";

export interface State {
  documentFormattings: DocumentFormatting[];
}

export const useDocumentFormattingStore = defineStore(
  "documentFormattingStore",
  {
    state: (): State => ({
      documentFormattings: [],
    }),
    getters: {},
    actions: {
      amendDocumentFormatting(
        id: string,
        payload: Partial<DocumentFormatting>
      ) {
        this.documentFormattings = this.documentFormattings.map((item) => {
          if (item.id === id) {
            return {
              ...item,
              ...payload,
            };
          }
          return item;
        });
      },
      async fetchDocumentFormattingsAsync() {
        const customerStore = useCustomerStore();
        const { fetchCustomerByEmailAsync } = customerStore;
        const { currentCustomer } = storeToRefs(customerStore);

        try {
          await fetchCustomerByEmailAsync(
            currentCustomer.value.email as string
          );

          const {
            documentFormattings,
          }: { documentFormattings: DocumentFormatting[] } =
            currentCustomer.value;

          this.documentFormattings = isEmpty(documentFormattings)
            ? StaticList.getDefaultDocumentFormattings()
            : documentFormattings;
        } catch (error) {
          return Promise.reject(error);
        }
      },
    },
  }
);
