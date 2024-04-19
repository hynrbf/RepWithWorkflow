import{_ as d}from"./ModalComponent.vue_vue_type_script_setup_true_lang-fe9b98ed.js";import{s as c}from"./organizationchart.esm-8c183601.js";import{j as p,bH as h,_ as m,aA as _,o as u,q as g,w as i,P as t,n as f,x as n}from"./index-9d6d511f.js";const x=p({name:"ARIndividualControllerChartModal",props:{registeredCustomer:{type:Object,default:new h}},data(){return{individualControllers:this.registeredCustomer.individualControllers,selection:{}}},components:{OrganizationChart:c},computed:{transformedData(){if(!(this.individualControllers.length>0))return{key:void 0};const e=this.individualControllers[0],s=this.individualControllers.slice(1);return{...this.mapEmployeeToOrganizationChartNode(e),children:s.map(o=>this.mapEmployeeToOrganizationChartNode(o))}}},methods:{mapEmployeeToOrganizationChartNode(e){return{key:e.detail.title,type:"person",data:{name:`${e.detail.forename} ${e.detail.surname}`,votingRights:`${e.detail.percentageOfVotingRights}`,ownership:`${e.detail.percentageOfCapital}`}}}}});const v={class:"d-flex flex-column",style:{"margin-top":"20px"}},C={class:"node-content"},y=["src"],w={class:"node-name"},O={class:"d-flex",style:{"justify-content":"start","margin-top":"15px"}},$={class:"d-flex flex-column",style:{width:"100px",height:"24px","margin-right":"30px"}},b={class:"text-value"},R=t("span",{class:"text-label"},"Voting Rights",-1),z=t("span",{style:{width:"0.5px",height:"40px",background:"var(--text-text-disabled)"}},null,-1),k={class:"d-flex flex-column",style:{width:"100px",height:"24px","margin-left":"30px"}},D={class:"text-value"},E=t("span",{class:"text-label"},"Ownership",-1);function M(e,s,o,N,A,V){const l=_("OrganizationChart"),r=d;return u(),g(r,{ref:"modalElement",width:"1031",height:"600",title:"Owners & Controllers Structure Chart",isShowDownloadPDF:!0},{default:i(()=>[t("div",v,[f(l,{selectionKeys:e.selection,"onUpdate:selectionKeys":s[0]||(s[0]=a=>e.selection=a),value:e.transformedData,collapsible:"",selectionMode:"multiple"},{person:i(a=>[t("div",C,[t("img",{alt:"individual-avatar",src:"/individual-avatar.png",class:"node-avatar"},null,8,y),t("span",w,n(a.node.data.name),1),t("div",O,[t("div",$,[t("span",b,n(a.node.data.votingRights),1),R]),z,t("div",k,[t("span",D,n(a.node.data.ownership),1),E])])])]),_:1},8,["selectionKeys","value"])])]),_:1},512)}const K=m(x,[["render",M]]);export{K as default};