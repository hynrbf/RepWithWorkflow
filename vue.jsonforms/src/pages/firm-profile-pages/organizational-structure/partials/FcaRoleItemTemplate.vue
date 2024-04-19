<script lang="ts">
import { defineComponent } from "vue";
import Role from "@/entities/org-structure/Role";
import { AppConstants } from "@/infra/AppConstants";

export default defineComponent({
  name: "FcaRoleItemTemplate",
  computed: {
    AppConstants() {
      return AppConstants;
    },
  },
  props: {
    role: {
      type: Object as () => Role,
      default: undefined,
    },
    clickEvent: {
      type: Object as () => (event: any) => void,
      default: undefined,
    },
    isMultiSelect: {
      type: Boolean,
      default: false,
    },
  },
  methods: {
    addRoleToFca(role: Role) {
      this.$emit("addRoleToFca", role);
    },

    toggleRoleIsPending(role: Role) {
      this.$emit("toggleRoleIsPending", role);
    },

    removeRoleFromFca(role: Role) {
      this.$emit("removeRoleFromFca", role);
    },
  },
});
</script>

<template>
  <div class="primary-role-dropdown-header">
    <div class="first-column">
      <div
        v-if="role"
        class="first-column"
        :style="
          role.isModified
            ? 'color: var(--color-warning-700)'
            : role?.isFcaAuthorised
            ? 'color: var(--color-success-600)'
            : ''
        "
        @click="clickEvent"
      >
        {{ role.name }}
      </div>
    </div>

    <div v-if="isMultiSelect" class="col d-flex align-items-center">
      <div v-if="role" class="col">
        <img
          :class="role.isFcaAuthorised ? 'authorised' : 'unauthorised'"
          :src="
            role.isFcaAuthorised
              ? '/check-filled.svg'
              : '/icons/clear-cicle-solid.svg'
          "
          :alt="
            role.isFcaAuthorised ? 'check-filled.svg' : 'clear-cicle-solid.svg'
          "
        />
      </div>

      <div v-if="role" class="col">
        <IconComponent
          v-if="role.isFcaAuthorised"
          symbol="add-square-53"
          size="20"
          :style="{ color: 'var(--text-text-disabled)' }"
        />

        <div v-else>
          <IconComponent
            v-if="role.state === AppConstants.removedState"
            symbol="add-square-53"
            size="20"
            :style="{
              color: 'var(--color-primary)',
            }"
            @click="addRoleToFca(role)"
          />

          <img
            v-else
            src="/add-square-primary-filled.svg"
            alt="/add-square-primary-filled.svg"
            @click="addRoleToFca(role)"
          />
        </div>
      </div>

      <div v-if="role" class="col">
        <div @click="toggleRoleIsPending(role)">
          <Checkbox
            style="pointer-events: none"
            :value="role.isPending"
            size="large"
          />
        </div>
      </div>

      <div v-if="role" class="col">
        <IconComponent
          size="18"
          :symbol="
            role.isFcaAuthorised
              ? 'subtract-square-26'
              : 'subtract-square-disabled'
          "
          :style="{ color: role.isFcaAuthorised ? 'var(--color-primary)' : '' }"
          @click="role.isFcaAuthorised ? removeRoleFromFca(role) : ''"
        />
      </div>
    </div>

    <div v-else class="col d-flex align-items-center">
      <div v-if="role" class="col">
        <img
          :class="role.isFcaAuthorised ? 'authorised' : 'unauthorised'"
          :src="
            role?.isFcaAuthorised
              ? '/check-filled.svg'
              : '/icons/clear-cicle-solid.svg'
          "
          :alt="
            role?.isFcaAuthorised ? 'check-filled.svg' : 'clear-cicle-solid.svg'
          "
        />
      </div>

      <div v-if="role" class="col">
        <IconComponent
          v-if="role.isFcaAuthorised"
          symbol="add-square-53"
          size="20"
          :style="{ color: 'var(--text-text-disabled)' }"
        />

        <div v-else>
          <IconComponent
            v-if="role.state === AppConstants.removedState"
            symbol="add-square-53"
            size="20"
            :style="{
              color: 'var(--color-primary)',
            }"
            @click="addRoleToFca(role)"
          />

          <img
            v-else
            src="/add-square-primary-filled.svg"
            alt="/add-square-primary-filled.svg"
            @click="addRoleToFca(role)"
          />
        </div>
      </div>

      <div v-if="role" class="col">
        <div @click="toggleRoleIsPending(role)">
          <Checkbox
            style="pointer-events: none"
            :value="role.isPending"
            size="medium"
          />
        </div>
      </div>

      <div v-if="role" class="col">
        <IconComponent
          size="18"
          :symbol="
            role.isFcaAuthorised
              ? 'subtract-square-26'
              : 'subtract-square-disabled'
          "
          :style="{ color: role.isFcaAuthorised ? 'var(--color-primary)' : '' }"
          @click="role.isFcaAuthorised ? removeRoleFromFca(role) : ''"
        />
      </div>
    </div>
  </div>
</template>

<style scoped>
.primary-role-dropdown-header {
  display: flex;
  align-items: center;
  flex-grow: 1;

  text-align: center;
  font-size: var(--font-size-xs);
  font-style: normal;
  font-weight: 600;
  line-height: 16px; /* 133.333% */
}

.primary-role-dropdown-header img.authorised {
  width: 16px;
  height: 16px;
}

.primary-role-dropdown-header img.unauthorised {
  width: 20px;
  height: 20px;
}

.first-column {
  width: 380px;
  text-align: start;
}
</style>