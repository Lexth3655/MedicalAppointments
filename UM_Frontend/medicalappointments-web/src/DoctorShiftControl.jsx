import { useMemo, useState } from 'react';
import { CalendarClock, Coffee, Pause, Play, RotateCcw, Square } from 'lucide-react';

const BASE='/backend/agenda';
const pad=n=>String(n).padStart(2,'0');
const isoDate=d=>`${d.getFullYear()}-${pad(d.getMonth()+1)}-${pad(d.getDate())}`;
function currentSchedule(schedules){const now=new Date();const date=isoDate(now);const time=`${pad(now.getHours())}:${pad(now.getMinutes())}:${pad(now.getSeconds())}`;return schedules.find(s=>String(s.fechaDisponible).slice(0,10)===date&&String(s.horaInicio).slice(0,8)<=time&&String(s.horaFin).slice(0,8)>time)||null}

export function shiftStatus(schedules,appointments){const current=currentSchedule(schedules);if(!current)return 'Fuera de turno';if(current.estadoTurno==='EnDescanso')return 'En descanso';if(current.estadoTurno==='Break')return 'En break';if(current.estadoTurno!=='EnTurno')return 'Programado';const now=new Date();return appointments.some(a=>String(a.medicoId)===String(current.medicoId)&&a.estado!=='Cancelada'&&new Date(a.fechaHora)<=now&&new Date(a.fechaHora).getTime()+30*60000>now.getTime())?'En consulta':'En turno'}

export default function DoctorShiftControl({schedules,token,allowed,onReload,onToast}){
 const [busy,setBusy]=useState(false);const current=useMemo(()=>currentSchedule(schedules),[schedules]);if(!allowed)return null;
 const scheduleId=current?.id??current?.horarioMedicoId??current?.horarioMedicoID;
 const update=async estadoTurno=>{if(!scheduleId){onToast?.('No se encontró el identificador del turno. Actualiza el cronograma y vuelve a intentarlo.');return}setBusy(true);try{const response=await fetch(`${BASE}/api/horarios-medicos/${scheduleId}/estado-turno`,{method:'PATCH',headers:{'Content-Type':'application/json',...(token?{Authorization:`Bearer ${token}`}:{})},body:JSON.stringify({estadoTurno})});if(!response.ok){let message=`No se pudo actualizar el turno (${response.status}).`;try{const body=await response.json();message=body.message||body.detail||message}catch{}if(response.status===404)message='La API de Agenda que está ejecutándose no reconoce la ruta para actualizar el estado del turno. Reinicia Agenda para cargar la versión actual.';throw new Error(message)}onToast?.(estadoTurno==='EnTurno'?'Turno iniciado':estadoTurno==='Finalizado'?'Turno finalizado':estadoTurno==='Break'?'Break iniciado':'Descanso iniciado');await onReload?.()}catch(e){onToast?.(e.message)}finally{setBusy(false)}};
 if(!current)return <div className="shift-actions"><button className="shift-control start" disabled title="Consulta el cronograma mensual para conocer tu próximo turno"><CalendarClock size={13}/> Sin turno ahora</button></div>;
 const state=current.estadoTurno||'Programado';
 if(state==='Programado')return <div className="shift-actions"><button className="shift-control resume" disabled={busy} onClick={()=>update('EnTurno')}><Play size={13}/> Iniciar turno</button></div>;
 if(state==='EnTurno')return <div className="shift-actions"><button className="shift-control pause" disabled={busy} onClick={()=>update('EnDescanso')} title="Marcar descanso"><Pause size={13}/> Descanso</button><button className="shift-control break" disabled={busy} onClick={()=>update('Break')} title="Tomar break"><Coffee size={13}/> Break</button><button className="shift-control finish" disabled={busy} onClick={()=>update('Finalizado')} title="Finalizar turno"><Square size={12}/> Finalizar</button></div>;
 return <div className="shift-actions"><button className="shift-control resume" disabled={busy} onClick={()=>update('EnTurno')} title="Reanudar turno"><RotateCcw size={13}/> Reanudar</button></div>;
}
