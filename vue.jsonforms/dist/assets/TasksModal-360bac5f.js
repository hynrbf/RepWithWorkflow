import{_ as h}from"./ModalComponent.vue_vue_type_script_setup_true_lang-fe9b98ed.js";import{j as x,C as g,G as k,o,q as y,w as n,P as e,n as a,x as s,H as c,m as _,J as C,K as D,r as N,bo as T,bp as b,bh as w,_ as B}from"./index-9d6d511f.js";const M={class:"d-flex modal-content"},E={class:"vstack"},V={class:"d-flex flex-row vstack align-center"},P={class:"provider-name"},$={class:"hstack mt-3"},j={class:"mt-2"},q=x({__name:"TasksModal",props:{tasks:{},providerName:{}},setup(l){const i=l,{tasks:r,providerName:d}=g(i),m=k();return(z,A)=>{const p=T,u=b,f=w,v=h;return o(),y(v,{ref_key:"modalElement",ref:m,title:"Tasks"},{default:n(()=>[e("div",M,[e("div",E,[a(p,{class:"vstack",type:"image",rounded:"full",size:"large"}),e("div",V,[e("text",P,s(c(d)),1)]),(o(!0),_(C,null,D(c(r),t=>(o(),_("div",null,[e("div",$,[e("div",null,"["+s(t.name)+"]",1),a(u,{themeColor:"success-tint",style:{"margin-left":"20px"}},{default:n(()=>[N(" Due by "+s(t.dueDate),1)]),_:2},1024)]),e("div",j,s(t.description),1),a(f,{height:1,width:550,spacing:0,style:{"margin-top":"20px"}})]))),256))])])]),_:1},512)}}});const H=B(q,[["__scopeId","data-v-483fea5e"]]);export{H as default};