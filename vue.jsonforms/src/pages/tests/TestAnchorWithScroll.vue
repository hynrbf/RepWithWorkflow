<script lang="ts">
import { defineComponent } from "vue";
import { container } from "tsyringe";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
//import {usePageSection} from "@/composables/usePageSection";

export default defineComponent({
  name: "TestAnchorWithScroll",
  data() {
    return {
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      activePageSectionId: "",
      pageSections: [
        { id: "firstTest", offsetTop: 0 },
        { id: "applyTest", offsetTop: 0 },
        { id: "lastTest", offsetTop: 0 },
      ],
    };
  },
  created() {},
  computed: {},
  mounted() {
    this.updatePageSectionTop();
    window.addEventListener("scroll", this.handleScroll);
  },
  methods: {
    updatePageSectionTop() {
      this.pageSections.forEach((section) => {
        const sectionElement = document.getElementById(section.id);
        section.offsetTop = sectionElement?.offsetTop ?? 0;
      });
    },

    handleScroll() {
      const scrollPosition = window.scrollY;

      if (!this.pageSections) {
        return;
      }

      for (let i = this.pageSections.length - 1; i >= 0; i--) {
        const section = this.pageSections[i];

        if (scrollPosition < section.offsetTop) {
          continue;
        }

        if (this.activePageSectionId === section.id) {
          break;
        }

        this.activePageSectionId = section.id;
        break;
      }
    },

    scrollToPageSection(sectionId: string) {
      const sectionElement = document.getElementById(sectionId);
      window.scrollTo({
        top: (sectionElement?.offsetTop ?? 0) + 1,
        behavior: "smooth",
      });
    },

    async onTogglePageSectionAccordionAsync(_isOpen: boolean) {
      await this.helperService.delayAsync(50);
      this.updatePageSectionTop();
    },
  },
});
</script>

<template>
  <StackLayout
    :align="{ horizontal: 'stretch' }"
    class="m-4"
    orientation="vertical"
    :gap="8"
  >
    <KendoAccordionItemComponent
      :isOpen="false"
      :isActive="activePageSectionId === 'firstTest'"
      id="firstTest"
      title="First Test"
      @onToggle="onTogglePageSectionAccordionAsync"
    >
      <p>
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas et
        diam vehicula, eleifend odio pharetra, placerat nibh. In dignissim felis
        augue, fringilla scelerisque felis sollicitudin faucibus. Praesent
        suscipit hendrerit lacus eleifend sodales. Fusce luctus consectetur
        convallis. Pellentesque habitant morbi tristique senectus et netus et
        malesuada fames ac turpis egestas. Mauris tempus quam sed neque blandit
        consectetur. Vestibulum ante ipsum primis in faucibus orci luctus et
        ultrices posuere cubilia curae; Nam sit amet commodo quam. Praesent ac
        tincidunt sapien. Nulla sit amet enim luctus, consectetur erat sed,
        accumsan mi. Quisque interdum orci nec nunc tincidunt, id posuere felis
        vehicula. Vivamus in erat purus. Nullam pellentesque quam a lobortis
        malesuada. Fusce gravida ultrices ornare. Aliquam commodo erat nec
        efficitur semper. Maecenas ut sodales eros. Donec bibendum sodales eros
        tristique tincidunt. Nam eu fermentum arcu. Fusce hendrerit vitae orci
        in placerat. Cras vel bibendum nisl. In sed interdum mauris. Fusce
        posuere purus sed nibh tempus, at imperdiet enim rutrum. Etiam ut mi
        ornare orci pharetra feugiat in et ipsum. Nam tempor finibus lorem.
        Morbi convallis aliquam tempor. Fusce tempus est nec fermentum eleifend.
        Curabitur nec scelerisque neque. Interdum et malesuada fames ac ante
        ipsum primis in faucibus. Donec eleifend mattis tellus eget interdum.
        Maecenas ornare urna sit amet erat iaculis, sed ullamcorper felis
        pellentesque. Cras et justo metus. Aenean vehicula mi id ligula dapibus,
        a accumsan nisl finibus. Sed pharetra tortor vel nisi molestie sodales.
        In porta neque pellentesque ante interdum mollis et sit amet eros. Nunc
        vel mi ut quam commodo volutpat. Ut a interdum lorem. Donec aliquet
        tempus justo, sed fermentum justo venenatis et. Maecenas maximus dui eu
        neque tempor suscipit. Nam rhoncus hendrerit arcu sed tempor. Cras nec
        tortor efficitur tortor venenatis hendrerit. Ut id arcu nec risus
        scelerisque elementum. Aliquam volutpat dapibus velit, sed ornare elit
        finibus et. Aliquam id euismod lacus, a auctor ex. Nunc lorem massa,
        viverra at dictum vel, lacinia sed lorem. Sed et dapibus eros. Interdum
        et malesuada fames ac ante ipsum primis in faucibus. Aenean gravida
        magna quis vestibulum pretium. Vestibulum ante ipsum primis in faucibus
        orci luctus et ultrices posuere cubilia curae; Aliquam consectetur
        rhoncus leo non ultricies. Morbi rhoncus arcu luctus luctus mattis. In
        dignissim, nisi quis ullamcorper ornare, ex nibh fermentum erat, quis
        sollicitudin lectus neque non nulla. Morbi in dapibus justo. Praesent
        scelerisque mi sapien, sit amet mollis quam iaculis quis. Suspendisse
        orci lacus, accumsan vel rutrum at, porta vitae enim. In interdum
        placerat justo nec pellentesque. Proin a sem sed risus accumsan
        condimentum pellentesque sed ante. Nulla porttitor in libero eget
        rhoncus. Donec fringilla vitae neque vitae facilisis. Phasellus velit
        ipsum, facilisis ut risus vitae, consequat pellentesque diam. Fusce
        hendrerit at magna a faucibus. Fusce eget risus in elit lacinia sodales.
        Maecenas feugiat nunc ut est rutrum malesuada. Ut lectus felis, gravida
        nec efficitur quis, venenatis id eros. Quisque consequat ut augue vel
        condimentum. Vestibulum et ligula eu diam aliquet condimentum. Donec a
        mi felis. Duis semper quam quis maximus rhoncus. Vestibulum sollicitudin
        dignissim eros nec imperdiet. Nullam nec lectus et lorem aliquam
        tincidunt. Curabitur aliquam eros sit amet magna ultricies, at
        ullamcorper justo tincidunt. Sed nec efficitur turpis, et tristique
        elit. Vestibulum id rutrum libero, viverra feugiat tellus. Fusce non
        odio at ipsum sodales fringilla. Sed sagittis enim eu porttitor blandit.
        Quisque ut nisi sodales, dignissim nisi quis, pellentesque nunc.
        Pellentesque at sagittis eros. Donec ornare tincidunt ullamcorper. Morbi
        cursus neque ac nunc bibendum condimentum et eget dui. Curabitur id
        turpis sagittis, semper massa nec, aliquet diam. Sed sit amet suscipit
        arcu. Praesent semper justo quis tellus placerat, in venenatis metus
        rutrum. Praesent consequat congue nulla dapibus maximus. Morbi dapibus
        porta mi, et facilisis diam molestie vel. Mauris ullamcorper eu diam
        condimentum dapibus. Quisque odio nisl, commodo placerat diam ut,
        maximus congue tellus. Etiam commodo sed arcu ut gravida. Proin ac
        aliquam quam. Orci varius natoque penatibus et magnis dis parturient
        montes, nascetur ridiculus mus. Fusce eu ligula laoreet, facilisis lacus
        tristique, dictum ante. Proin euismod lobortis elit, a dictum metus
        venenatis sit amet. Maecenas fermentum, risus non porttitor dictum, nisl
        nunc mollis leo, non elementum metus leo a ipsum. Integer elementum
        rhoncus augue sed eleifend. Quisque non turpis lectus. Vivamus purus mi,
        varius vitae ante ut, dictum facilisis nibh. Vivamus sit amet enim
        molestie, iaculis lacus at, ultricies neque. Sed ac enim sed sapien
        aliquet vulputate. Sed nec vehicula risus, vitae malesuada est. Morbi
        convallis in risus eu dictum. Suspendisse pellentesque tellus id enim
        imperdiet lobortis. Duis in nisi diam. Cras eu neque ac dolor porttitor
        pellentesque. Proin venenatis vehicula nisi tincidunt accumsan. Aenean
        eu cursus mauris, sit amet consequat neque. Nam vitae urna accumsan,
        vehicula velit sed, molestie orci. Suspendisse cursus bibendum blandit.
        Duis aliquet risus et massa imperdiet aliquet. Orci varius natoque
        penatibus et magnis dis parturient montes, nascetur ridiculus mus. Sed
        finibus suscipit libero sed sagittis.
      </p>
    </KendoAccordionItemComponent>

    <KendoAccordionItemComponent
      :isOpen="false"
      id="applyTest"
      title="Apply Now"
      :isActive="activePageSectionId === 'applyTest'"
      @onToggle="onTogglePageSectionAccordionAsync"
    >
      <p>
        Proin id neque laoreet, fermentum sem in, varius libero. Etiam vel
        finibus ante. Morbi et libero non sem molestie blandit vitae porta est.
        Etiam malesuada orci felis, vel eleifend nibh hendrerit in. Maecenas ac
        eros ac nunc congue efficitur. Fusce ut nisl vitae justo egestas
        molestie. Integer placerat mi at mauris congue, pellentesque mollis
        augue faucibus. Orci varius natoque penatibus et magnis dis parturient
        montes, nascetur ridiculus mus. Donec sem purus, euismod sed volutpat
        id, condimentum cursus orci. In at mi convallis, semper tellus ac,
        vestibulum leo. Cras scelerisque quis risus a condimentum. Morbi in
        molestie nibh. Phasellus vehicula velit vel sodales eleifend. Orci
        varius natoque penatibus et magnis dis parturient montes, nascetur
        ridiculus mus. Ut vel urna vitae augue feugiat facilisis a at arcu.
        Morbi consectetur ex eget odio cursus viverra. Aliquam erat volutpat.
        Maecenas tincidunt, odio sed congue congue, ipsum mauris commodo quam,
        ut porttitor magna ex vitae est. Nam vel arcu at augue egestas
        scelerisque. Vestibulum et posuere nibh. Etiam vestibulum laoreet dolor,
        quis pellentesque metus aliquet eget. Maecenas tempus iaculis magna, non
        semper justo convallis et. Vestibulum ut urna at elit mattis lobortis
        quis at erat. Quisque dui ligula, facilisis at imperdiet at, facilisis
        ut lorem. Vivamus ultrices orci ut tempus blandit. Curabitur quam leo,
        interdum et sollicitudin ac, pulvinar bibendum quam. Praesent eleifend
        nisi id nisi commodo mattis. Pellentesque congue, elit non bibendum
        fermentum, felis nulla tempus sapien, vel placerat augue purus tristique
        augue. Proin vel odio egestas, fermentum ligula eu, facilisis magna.
        Aliquam id rutrum urna. Etiam viverra ac urna in viverra. Etiam vel
        ultrices lacus, eget varius orci. Nullam urna mi, feugiat in quam sit
        amet, pretium ornare nisi. Nullam vestibulum diam eu malesuada laoreet.
        Fusce molestie enim a risus iaculis feugiat. Maecenas ex ipsum, interdum
        non dolor a, ullamcorper consequat orci. Sed tristique quam lectus, quis
        ultrices ante porttitor sed. Etiam justo sem, pellentesque eget pharetra
        non, lacinia interdum magna. Praesent dignissim non urna nec rutrum.
        Nulla tincidunt felis tristique pharetra pulvinar. Morbi sed sapien
        lobortis, vestibulum massa vitae, porttitor felis. In ac rutrum leo,
        vitae volutpat enim. Class aptent taciti sociosqu ad litora torquent per
        conubia nostra, per inceptos himenaeos. Mauris vulputate gravida turpis
        vitae tempus. Fusce gravida accumsan urna, at convallis nunc venenatis
        consectetur. Curabitur efficitur, velit vitae aliquet tempor, justo
        libero lobortis dui, sed vestibulum libero risus id turpis. Morbi varius
        congue massa, sit amet faucibus justo viverra ac. Etiam ut consequat
        nisi. Mauris urna nunc, congue at lacinia vel, hendrerit eget justo.
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum
        lacinia scelerisque dignissim. Vivamus gravida erat a ipsum maximus
        ultricies. Nam sed augue non ante feugiat suscipit. Ut rhoncus, ex vitae
        ullamcorper gravida, quam magna ultricies ligula, nec scelerisque quam
        urna ut ante. Lorem ipsum dolor sit amet, consectetur adipiscing elit.
      </p>
    </KendoAccordionItemComponent>

    <KendoAccordionItemComponent
      :isOpen="false"
      id="lastTest"
      title="Last Test"
      :isActive="activePageSectionId === 'lastTest'"
      @onToggle="onTogglePageSectionAccordionAsync"
    >
      <p>
        Proin id neque laoreet, fermentum sem in, varius libero. Etiam vel
        finibus ante. Morbi et libero non sem molestie blandit vitae porta est.
        Etiam malesuada orci felis, vel eleifend nibh hendrerit in. Maecenas ac
        eros ac nunc congue efficitur. Fusce ut nisl vitae justo egestas
        molestie. Integer placerat mi at mauris congue, pellentesque mollis
        augue faucibus. Orci varius natoque penatibus et magnis dis parturient
        montes, nascetur ridiculus mus. Donec sem purus, euismod sed volutpat
        id, condimentum cursus orci. In at mi convallis, semper tellus ac,
        vestibulum leo. Cras scelerisque quis risus a condimentum. Morbi in
        molestie nibh. Phasellus vehicula velit vel sodales eleifend. Orci
        varius natoque penatibus et magnis dis parturient montes, nascetur
        ridiculus mus. Ut vel urna vitae augue feugiat facilisis a at arcu.
        Morbi consectetur ex eget odio cursus viverra. Aliquam erat volutpat.
        Maecenas tincidunt, odio sed congue congue, ipsum mauris commodo quam,
        ut porttitor magna ex vitae est. Nam vel arcu at augue egestas
        scelerisque. Vestibulum et posuere nibh. Etiam vestibulum laoreet dolor,
        quis pellentesque metus aliquet eget. Maecenas tempus iaculis magna, non
        semper justo convallis et. Vestibulum ut urna at elit mattis lobortis
        quis at erat. Quisque dui ligula, facilisis at imperdiet at, facilisis
        ut lorem. Vivamus ultrices orci ut tempus blandit. Curabitur quam leo,
        interdum et sollicitudin ac, pulvinar bibendum quam. Praesent eleifend
        nisi id nisi commodo mattis. Pellentesque congue, elit non bibendum
        fermentum, felis nulla tempus sapien, vel placerat augue purus tristique
        augue. Proin vel odio egestas, fermentum ligula eu, facilisis magna.
        Aliquam id rutrum urna. Etiam viverra ac urna in viverra. Etiam vel
        ultrices lacus, eget varius orci. Nullam urna mi, feugiat in quam sit
        amet, pretium ornare nisi. Nullam vestibulum diam eu malesuada laoreet.
        Fusce molestie enim a risus iaculis feugiat. Maecenas ex ipsum, interdum
        non dolor a, ullamcorper consequat orci. Sed tristique quam lectus, quis
        ultrices ante porttitor sed. Etiam justo sem, pellentesque eget pharetra
        non, lacinia interdum magna. Praesent dignissim non urna nec rutrum.
        Nulla tincidunt felis tristique pharetra pulvinar. Morbi sed sapien
        lobortis, vestibulum massa vitae, porttitor felis. In ac rutrum leo,
        vitae volutpat enim. Class aptent taciti sociosqu ad litora torquent per
        conubia nostra, per inceptos himenaeos. Mauris vulputate gravida turpis
        vitae tempus. Fusce gravida accumsan urna, at convallis nunc venenatis
        consectetur. Curabitur efficitur, velit vitae aliquet tempor, justo
        libero lobortis dui, sed vestibulum libero risus id turpis. Morbi varius
        congue massa, sit amet faucibus justo viverra ac. Etiam ut consequat
        nisi. Mauris urna nunc, congue at lacinia vel, hendrerit eget justo.
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum
        lacinia scelerisque dignissim. Vivamus gravida erat a ipsum maximus
        ultricies. Nam sed augue non ante feugiat suscipit. Ut rhoncus, ex vitae
        ullamcorper gravida, quam magna ultricies ligula, nec scelerisque quam
        urna ut ante. Lorem ipsum dolor sit amet, consectetur adipiscing elit.
      </p>
    </KendoAccordionItemComponent>

    <div class="d-flex flex-row gap-4">
      <div
        class="col p-2"
        :class="
          activePageSectionId === section.id
            ? 'active-section'
            : 'inactive-section'
        "
        @click="scrollToPageSection(section.id)"
        v-for="section of pageSections"
      >
        {{ section.id }}
      </div>
    </div>

    <div v-for="_n in 50">&nbsp;</div>
  </StackLayout>
</template>

<style scoped>
.active-section {
  background-color: var(--color-primary);
  color: white;
}

.inactive-section {
  background-color: white;
  color: var(--color-primary);
}
</style>
