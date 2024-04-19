<script setup lang="ts">
import { StationeryEntity } from "@/entities/stationery/StationeryEntity";

defineProps<{ stationery?: StationeryEntity; isView?: boolean }>();

const emits = defineEmits<{
  (event: "update", value: StationeryEntity): void;
}>();

const deleteFile = (fileId: string, values: StationeryEntity) => {
  if (values.files) {
    values.files = values?.files.filter((file) => file.id !== fileId);
  }

  emits("update", {
    ...values,
  });
};
</script>

<template>
  <ModalComponent
    width="600"
    height="360"
    :hasIconTitle="true"
    :iconTitle="stationery?.icon"
    :title="stationery?.name"
    class="view-stationery"
  >
    <div class="text-center">
      <div class="file-container" v-if="isView">
        <ScrollView
          v-if="stationery && stationery?.files && stationery?.files.length > 1"
          :style="{
            width: '560px',
            height: '240px',
          }"
          :data-items="stationery?.files"
          :content="'content'"
          :endless="true"
        >
          <template #content="{ props }">
            <div class="image-container">
              <img
                :src="props.item.url"
                :alt="`File ${props.item.name}`"
                :draggable="false"
              />
            </div>
            <div class="file-list">
              <img
                :src="`/file-${props.item.extension}-v2.svg`"
                width="23"
                height="24"
              />
              <span class="document-name">{{
                `${props.item.name}.${props.item.extension}`
              }}</span>
              <a
                :href="`${props.item.url}`"
                target="_blank"
                class="k-button k-button-sm k-button-square k-rounded-full k-button-solid k-button-solid-light action-button"
              >
                <span class="k-button-text">
                  <IconComponent symbol="download-box-2-19" size="20" />
                </span>
              </a>
              <KendoButton
                type="button"
                size="small"
                rounded="full"
                shape="square"
                theme-color="light"
                title="Delete"
                class="action-button"
                @click="
                  stationery && deleteFile(props?.item.id ?? '', stationery)
                "
              >
                <IconComponent symbol="trash" class="text-error" size="20" />
              </KendoButton>
            </div>
          </template>
        </ScrollView>
        <div v-else>
          <div class="image-container">
            <img
              :src="stationery?.files?.[0]?.url || ''"
              :alt="`File ${stationery?.files?.[0]?.name ?? ''}`"
            />
          </div>
          <div
            class="file-list"
            v-for="(item, index) in stationery?.files"
            :key="index"
          >
            <img
              :src="`/file-${item.extension}-v2.svg`"
              width="23"
              height="24"
            />
            <span class="document-name">{{
              `${item.name}.${item.extension}`
            }}</span>
            <a
              :href="`${item.url}`"
              target="_blank"
              class="k-button k-button-sm k-button-square k-rounded-full k-button-solid k-button-solid-light action-button"
            >
              <span class="k-button-text">
                <IconComponent symbol="download-box-2-19" size="20" />
              </span>
            </a>
            <KendoButton
              type="button"
              size="small"
              rounded="full"
              shape="square"
              theme-color="light"
              title="Delete"
              class="action-button"
              @click="stationery && deleteFile(item?.id ?? '', stationery)"
            >
              <IconComponent symbol="trash" class="text-error" size="20" />
            </KendoButton>
          </div>
        </div>
      </div>

      <div v-else>
        <div
          class="file-list"
          v-for="(item, index) in stationery?.files"
          :key="index"
        >
          <img :src="`/file-${item.extension}-v2.svg`" width="23" height="24" />
          <span class="document-name">{{
            `${item.name}.${item.extension}`
          }}</span>
          <a
            :href="`${item.url}`"
            target="_blank"
            class="k-button k-button-sm k-button-square k-rounded-full k-button-solid k-button-solid-light action-button"
          >
            <span class="k-button-text">
              <IconComponent symbol="download-box-2-19" size="20" />
            </span>
          </a>
        </div>
      </div>
    </div>
    <template #footer>
      <div class="text-center justify-content-center">
        <StatusLabelComponent
          v-if="
            stationery && stationery?.files && stationery?.files.length <= 1
          "
          :status="stationery.status"
        />
      </div>
    </template>
  </ModalComponent>
</template>
<style lang="scss">
.view-stationery {
  .file-container {
    width: 100%;
    height: 156px;
  }
  .file-list {
    width: 100%;
    height: 40px;
    padding: 4px 9px;
    gap: 0px;
    border-radius: 8px;
    border: 0.5px solid #97a1af;
    position: relative;
    display: flex;
    vertical-align: middle;
    margin-top: 9px;

    img {
      float: left;
      margin: 4px 0px 2px;
    }

    .document-name {
      font-size: var(--font-size-default);
      color: var(--color-primary);
      font-weight: var(--font-weight-semi-bold);
      line-height: 20px;
      text-align: left;
      padding: 6px;
      width: 89%;
    }

    .action-button {
      width: 30px;
      height: 30px;
      color: var(--color-primary);
      float: right;
      margin: 0px 2px;
    }

    .action-button:last-child {
      margin-right: 0px;
    }
  }

  .k-scrollview {
    margin: 0 auto;
    border: none;
  }

  .image-container {
    position: relative;
    width: 480px;
    border: 1px solid #dfe8f5;
    border-radius: 8px;
    margin: 0 auto;

    img {
      width: 478px;
      height: 154px;
      margin: 0 auto;
      border-radius: 8px;
      object-fit: cover;
    }
  }

  .slide-arrows {
    position: absolute;
    top: 50%;
    width: 560px;
  }
}

.k-dialog-content {
  overflow: hidden;
}

.k-scrollview-prev,
.k-scrollview-next {
  position: absolute;
  cursor: pointer;
  background-color: #e4e7ec;
  color: white;
  padding: 5px;
  width: 30px;
  height: 30px;
  border-radius: 50%;
  opacity: 1;
  top: 25%;
}

.k-scrollview-next .k-svg-icon,
.k-scrollview-prev .k-svg-icon {
  width: 30px;
  height: 20px;
  color: var(--color-primary);
}

.k-scrollview-nav-wrap {
  padding: 0px;
  height: 24px;
  overflow: hidden;

  .k-scrollview-nav {
    padding: 0;
    margin: 0;
    height: 38px;

    .k-link {
      width: 14px;
      height: 14px;

      &::before {
        width: 14px;
        height: 14px;
      }
    }
  }
}
</style>
