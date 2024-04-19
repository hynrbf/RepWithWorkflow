import { defineStore, storeToRefs } from "pinia";
import { useArCustomerStore } from "@/stores/useArCustomerStore";
import { MediaMarketingOutlet } from "@/entities/media-marketing-outlet/MediaMarketingOutlet";
import { v4 as uuid } from "uuid";

export interface State {
  mediaMarketingOutlet: MediaMarketingOutlet | undefined;
  mediaMarketingOutlets: MediaMarketingOutlet[];
}

export const useArMediaMarketingOutletStore = defineStore(
  "arMediaMarketingOutletStore",
  {
    state: (): State => ({
      mediaMarketingOutlet: undefined,
      mediaMarketingOutlets: [],
    }),
    getters: {
      mediaMarketingOutletsUnarchived({
        mediaMarketingOutlets,
      }): MediaMarketingOutlet[] {
        return mediaMarketingOutlets.filter((item) => !item.archived);
      },
      mediaMarketingOutletsArchived({ mediaMarketingOutlets }) {
        return mediaMarketingOutlets.filter((item) => item.archived);
      },
    },
    actions: {
      setMediaMarketingOutlet(payload: MediaMarketingOutlet) {
        this.mediaMarketingOutlet = payload;
      },
      getMediaMarketingOutlet(id: string, cache = true) {
        const mediaMarketingOutlet = this.mediaMarketingOutlets.find(
          (item) => item.id === id
        );
        if (cache) {
          this.mediaMarketingOutlet = mediaMarketingOutlet;
        }
        return mediaMarketingOutlet;
      },
      appendMediaMarketingOutlet(item: MediaMarketingOutlet) {
        this.mediaMarketingOutlets = [...this.mediaMarketingOutlets, item];
        return item;
      },
      amendMediaMarketingOutlet(
        id: string,
        payload: Partial<MediaMarketingOutlet>
      ) {
        let amendedItem: MediaMarketingOutlet | undefined;
        this.mediaMarketingOutlets = this.mediaMarketingOutlets.map((item) => {
          if (item.id === id) {
            amendedItem = {
              ...item,
              ...payload,
            };
            return amendedItem;
          }
          return item;
        });
        return amendedItem;
      },
      removeMediaMarketingOutlet(id: string) {
        this.mediaMarketingOutlets = this.mediaMarketingOutlets.filter(
          (item) => item.id !== id
        );
      },
      async fetchMediaMarketingOutletsAsync() {
        const arCustomerStore = useArCustomerStore();
        const { fetchArCustomerByEmailAsync } = arCustomerStore;
        const { currentArCustomer } = storeToRefs(arCustomerStore);

        try {
          await fetchArCustomerByEmailAsync(
            currentArCustomer.value.email as string
          );

          const {
            mediaMarketingOutlets,
          }: { mediaMarketingOutlets: MediaMarketingOutlet[] } =
            currentArCustomer.value;

          this.mediaMarketingOutlets = mediaMarketingOutlets;
        } catch (error) {
          return Promise.reject(error);
        }
      },
      async fetchMediaMarketingOutletAsync(
        id: string,
        cache = true
      ): Promise<MediaMarketingOutlet> {
        const arCustomerStore = useArCustomerStore();
        const { fetchArCustomerByEmailAsync } = arCustomerStore;
        const { currentArCustomer } = storeToRefs(arCustomerStore);
        try {
          await fetchArCustomerByEmailAsync(
            currentArCustomer.value.email as string
          );
          const response = currentArCustomer.value.mediaMarketingOutlets?.find(
            (item) => item.id === id
          );
          if (cache) {
            this.mediaMarketingOutlet = response;
          }
          if (!response) {
            throw new Error("Media Marketing not found.");
          }
          return Promise.resolve(response);
        } catch (error) {
          return Promise.reject(error);
        }
      },
      async createMediaMarketingOutletAsync(
        payload: Partial<MediaMarketingOutlet>,
        cache = true
      ): Promise<MediaMarketingOutlet> {
        const arCustomerStore = useArCustomerStore();
        const { updateCustomerArByEmailAsync } = arCustomerStore;
        const { currentArCustomer } = storeToRefs(arCustomerStore);

        try {
          const email = currentArCustomer.value.email;
          const mediaMarketingOutlets =
            currentArCustomer.value.mediaMarketingOutlets || [];

          if (!email) {
            throw new Error("Email not defined.");
          }

          const newItem: MediaMarketingOutlet = {
            id: uuid(),
            createdAt: Date.now(),
            ...payload,
          };

          await updateCustomerArByEmailAsync(email, {
            mediaMarketingOutlets: [...mediaMarketingOutlets, newItem],
          });

          if (cache) {
            this.appendMediaMarketingOutlet(newItem);
          }
          return Promise.resolve(newItem);
        } catch (error) {
          return Promise.reject(error);
        }
      },
      async updateMediaMarketingOutletAsync(
        id: string,
        payload: Partial<MediaMarketingOutlet>,
        cache = true
      ): Promise<MediaMarketingOutlet> {
        const arCustomerStore = useArCustomerStore();
        const { updateCustomerArByEmailAsync } = arCustomerStore;
        const { currentArCustomer } = storeToRefs(arCustomerStore);

        try {
          const email = currentArCustomer.value.email;
          const mediaMarketingOutlets =
            currentArCustomer.value.mediaMarketingOutlets || [];

          if (!email) {
            throw new Error("Email not defined.");
          }

          let updatedItem: MediaMarketingOutlet | undefined;

          await updateCustomerArByEmailAsync(email, {
            mediaMarketingOutlets: [
              ...mediaMarketingOutlets.map((item) => {
                if (item.id === id) {
                  return (updatedItem = {
                    ...item,
                    ...payload,
                  });
                }
                return item;
              }),
            ],
          });

          if (!updatedItem) {
            throw new Error("Media Marketing not found.");
          }

          if (cache) {
            this.mediaMarketingOutlet = updatedItem;
            this.amendMediaMarketingOutlet(updatedItem.id, updatedItem);
          }
          return Promise.resolve(updatedItem);
        } catch (error) {
          return Promise.reject(error);
        }
      },
      async deleteMediaMarketingOutletAsync(
        id: string,
        cache = true
      ): Promise<boolean> {
        const arCustomerStore = useArCustomerStore();
        const { updateCustomerArByEmailAsync } = arCustomerStore;
        const { currentArCustomer } = storeToRefs(arCustomerStore);

        try {
          const email = currentArCustomer.value.email;
          const mediaMarketingOutlets =
            currentArCustomer.value.mediaMarketingOutlets || [];

          if (!email) {
            throw new Error("Email not defined.");
          }

          await updateCustomerArByEmailAsync(email, {
            mediaMarketingOutlets: [
              ...mediaMarketingOutlets.filter((item) => item.id !== id),
            ],
          });

          if (cache) {
            this.removeMediaMarketingOutlet(id);
          }

          return Promise.resolve(true);
        } catch (error) {
          return Promise.reject(error);
        }
      },
    },
  }
);
