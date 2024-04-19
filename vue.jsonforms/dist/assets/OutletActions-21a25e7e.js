import{_ as p}from"./SpinnerComponent-154f27a2.js";import{j as v,bf as _,o as n,m as h,n as s,w as r,q as u,aB as y,$ as f,bg as b,N as g,O as B,_ as k}from"./index-9d6d511f.js";const w={class:"d-inline-flex gap-2"},z=v({__name:"OutletActions",props:{outlet:{},archiving:{type:Boolean},archiveItem:{}},emits:["view","archive","remove"],setup($,{emit:l}){return(e,t)=>{const i=g,a=B,d=p,m=_("dev-only");return n(),h("div",w,[s(a,{type:"button",size:"small",rounded:"full",shape:"square","theme-color":"light",title:"View",class:"ActionButton",onClick:t[0]||(t[0]=o=>l("view",`${e.outlet.id}`))},{default:r(()=>[s(i,{symbol:"eye",size:"20"})]),_:1}),s(a,{type:"button",size:"small",rounded:"full",shape:"square","theme-color":"light",title:"Upload",class:"ActionButton",onClick:t[1]||(t[1]=f(o=>{var c;return l("archive",`${e.outlet.id}`,!((c=e.outlet)!=null&&c.archived))},["prevent"]))},{default:r(()=>{var o;return[e.archiving&&e.archiveItem===e.outlet.id?(n(),u(d,{key:0,size:20,class:"mt-2"})):(n(),u(i,{key:1,symbol:e.outlet.archived?"browser-refresh-66":"archived",size:"18",style:y({color:(o=e.outlet)!=null&&o.archived?"var(--color-success)":null})},null,8,["symbol","style"]))]}),_:1}),b((n(),u(a,{type:"button",size:"small",rounded:"full",shape:"square","theme-color":"light",title:"Remove",class:"ActionButton",onClick:t[2]||(t[2]=o=>l("remove",`${e.outlet.id}`))},{default:r(()=>[s(i,{symbol:"trash-bin-remove-6",size:"20"})]),_:1})),[[m]])])}}});const q=k(z,[["__scopeId","data-v-d3ca7496"]]);export{q as default};
