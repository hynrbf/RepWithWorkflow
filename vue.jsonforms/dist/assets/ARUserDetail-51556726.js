import{j as b,bo as A,cE as N,_ as w,aA as u,o as d,m as r,n as t,w as a,P as i,r as l,x as s,a0 as C,q as S,y as g,J as y,K as k,L as D,N as B,O as R,bp as W,S as L,bh as M}from"./index-9d6d511f.js";import z from"./ARRoleDetails-f95d3a3e.js";const $=b({name:"ARUserDetail",components:{ARRoleDetails:z,DynamicAvatar:A},props:{employee:{type:Object,default:new N},dialogWidth:{type:Number,default:330}},methods:{getStatusPillColor(e){return e?e==="Active"?"good-status-pill":e==="Onboarding"?"warning-status-pill":"bad-status-pill":""}}}),P={class:"d-flex align-items-center"},V={class:"d-flex my-2"},E={class:"col d-flex flex-column gap-2"},T={class:"col d-flex flex-column gap-2"},O={class:"col d-flex flex-column gap-2"},j={key:1,class:"d-flex flex-column my-2"},q=i("text",{class:"detail-dialog-section-text"},"Line Manager",-1),F={class:"d-flex",style:{"margin-top":"5px"}},K={class:"detail-dialog-text-primary",style:{"margin-top":"8px"}},U={class:"vstack my-3",style:{gap:"5px"}},I={class:"d-flex mt-2"},J={class:"col vstack"},G={class:"col vstack"};function H(e,Q,X,Y,Z,ee){const c=u("DynamicAvatar"),o=D,_=B,f=R,h=W,v=L,m=M,x=u("ARRoleDetails");return d(),r(y,null,[t(v,{orientation:"vertical",style:{"margin-bottom":"5px"},gap:10,align:{horizontal:"center",vertical:"top"}},{default:a(()=>{var n,p;return[i("div",null,[t(c,{size:"large",type:"image",rounded:"full",customStyle:{flexBasis:"60px",height:"60px"},imageSrc:(n=e.employee)==null?void 0:n.img_id,imageAlt:(p=e.employee)==null?void 0:p.img_id},null,8,["imageSrc","imageAlt"])]),i("div",P,[t(o,{class:"detail-dialog-employee-name"},{default:a(()=>[l(s(e.employee.firstName)+" "+s(e.employee.lastName),1)]),_:1}),t(f,{style:{height:"25px",width:"25px","margin-left":"10px","margin-top":"1px"},type:"button",size:"small",rounded:"full",shape:"square","theme-color":"light",title:"View",class:"ActionButton"},{default:a(()=>[t(_,{symbol:"edit-pen",size:"20",color:"var(--brand-color-brand-primary)"})]),_:1})]),t(h,{class:C(e.getStatusPillColor(e.employee.employmentStatus))},{default:a(()=>[l(s(e.employee.employmentStatus),1)]),_:1},8,["class"])]}),_:1}),t(m,{class:"my-2",height:1,width:e.dialogWidth-60,spacing:0},null,8,["width"]),i("div",V,[i("div",E,[t(o,{class:"detail-dialog-section-text"},{default:a(()=>[l("Title")]),_:1}),t(o,{class:"detail-dialog-text-primary"},{default:a(()=>[l(s(e.employee.title),1)]),_:1})]),i("div",T,[t(o,{class:"detail-dialog-section-text"},{default:a(()=>[l("Forename(s)")]),_:1}),t(o,{class:"detail-dialog-text-primary"},{default:a(()=>[l(s(e.employee.firstName),1)]),_:1})]),i("div",O,[t(o,{class:"detail-dialog-section-text"},{default:a(()=>[l("Surname")]),_:1}),t(o,{class:"detail-dialog-text-primary"},{default:a(()=>[l(s(e.employee.lastName),1)]),_:1})])]),e.employee.lineManager?(d(),S(m,{key:0,class:"my-2",height:1,width:e.dialogWidth-60,spacing:0},null,8,["width"])):g("",!0),e.employee.lineManager?(d(),r("div",j,[q,i("div",F,[t(c,{type:"image",rounded:"full",size:"medium",imageSrc:e.employee.lineManager.img_id,imageAlt:e.employee.lineManager.img_id},null,8,["imageSrc","imageAlt"]),i("text",K,s(e.employee.lineManager.firstName)+" "+s(e.employee.lineManager.lastName),1)])])):g("",!0),t(m,{class:"my-2",height:1,width:e.dialogWidth-60,spacing:0},null,8,["width"]),t(x,{employee:e.employee,dialogWidth:e.dialogWidth},null,8,["employee","dialogWidth"]),i("div",U,[t(o,{class:"detail-dialog-section-text"},{default:a(()=>[l("Product Type")]),_:1}),(d(!0),r(y,null,k(e.employee.productType,n=>(d(),r("div",{key:n.value},[t(o,{class:"detail-dialog-text-primary"},{default:a(()=>[l(s(n.value),1)]),_:2},1024)]))),128))]),t(m,{class:"my-2",height:1,width:e.dialogWidth-60,spacing:0},null,8,["width"]),i("div",I,[i("div",J,[t(o,{class:"detail-dialog-section-text"},{default:a(()=>[l("Email Address")]),_:1}),t(o,{class:"detail-dialog-text-primary"},{default:a(()=>[l(s(e.employee.email),1)]),_:1})]),i("div",G,[t(o,{class:"detail-dialog-section-text"},{default:a(()=>[l("Contact Number")]),_:1}),t(o,{class:"detail-dialog-text-primary"},{default:a(()=>[l(s(e.employee.contactNumber.dialCode)+" "+s(e.employee.contactNumber.number),1)]),_:1})])])],64)}const le=w($,[["render",H]]);export{le as default};
