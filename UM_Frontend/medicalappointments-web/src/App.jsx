import { useCallback, useEffect, useMemo, useState } from 'react';
import { Activity, ArrowDownRight, ArrowUpRight, CalendarDays, ChevronDown, ClipboardList, Clock3, HeartPulse, Plus, Search, ShieldCheck, Stethoscope, Users, X, UserRound, KeyRound, Pencil } from 'lucide-react';
import Maintenance from './Maintenance';
import AppointmentWorkspace from './AppointmentWorkspace';
import EmergencyContacts from './EmergencyContacts';
import PatientRecordModal from './PatientRecordModal';
import DoctorShiftControl, { shiftStatus } from './DoctorShiftControl';
import AgendaCatalog from './AgendaCatalog';
import ClinicalNotesBoard from './ClinicalNotesBoard';
import ClinicalRecordsBoard from './ClinicalRecordsBoard';
import MonthlyScheduleBoard from './MonthlyScheduleBoard';
import ClinicalOverviewBoard from './ClinicalOverviewBoard';

const API = {
  security: import.meta.env.VITE_SECURITY_API_URL || '/backend/security',
  patients: import.meta.env.VITE_PATIENTS_API_URL || '/backend/patients',
  agenda: import.meta.env.VITE_AGENDA_API_URL || '/backend/agenda',
  clinical: import.meta.env.VITE_CLINICAL_API_URL || '/backend/clinical-care',
};

async function request(baseUrl, path, { token, ...options } = {}) {
  const response = await fetch(`${baseUrl}${path}`, {
    ...options,
    headers: {
      ...(options.body ? { 'Content-Type': 'application/json' } : {}),
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options.headers,
    },
  });
  if (!response.ok) {
    let detail = `${response.status} ${response.statusText}`;
    try {
      const body = await response.json();
      detail = body.message || body.detail || detail;
    } catch { /* API returned no JSON body */ }
    throw new Error(detail);
  }
  if (response.status === 204) return null;
  return response.json();
}

const rowsOf = value => Array.isArray(value) ? value : value?.data || value?.items || value?.result || [];
const initials = name => (name || 'Usuario').split(' ').slice(0, 2).map(part => part[0]).join('').toUpperCase();
const displayDate = value => value ? new Date(value).toLocaleDateString('es-SV', { day: '2-digit', month: 'short', year: 'numeric' }) : '—';
const displayTime = value => value ? new Date(value).toLocaleTimeString('es-SV', { hour: '2-digit', minute: '2-digit' }) : '—';
function readPermissions(token) {
  try { const payload=JSON.parse(atob(token.split('.')[1].replace(/-/g,'+').replace(/_/g,'/'))); const raw=payload.perm||[]; return new Set(Array.isArray(raw)?raw:[raw]); }
  catch { return new Set(); }
}
function Login({ onLogin, notice }) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');
  const submit = async event => {
    event.preventDefault(); setBusy(true); setError('');
    try {
      const session = await request(API.security, '/api/auth/login', { method: 'POST', body: JSON.stringify({ username, password }) });
      onLogin(session);
    } catch (exception) { setError(exception.message); }
    finally { setBusy(false); }
  };
  return <main className="login-shell">
    <section className="login-art">
      <div className="brand"><div className="brand-mark"><HeartPulse size={22} /></div><span>medical<span className="brand-light">appointments</span></span></div>
      <div className="art-copy"><span className="eyebrow">GESTIÓN CLÍNICA</span><h1>Una atención<br />más humana.</h1><p>Todo lo esencial de tu clínica, organizado en un solo lugar.</p></div>
      <div className="art-card"><div className="art-card-icon"><Activity size={18} /></div><div><strong>Atención conectada</strong><span>Personas, agenda y equipo en sincronía</span></div><span className="pulse-dot" /></div>
      <div className="login-orb orb-one" /><div className="login-orb orb-two" />
    </section>
    <section className="login-panel"><div className="login-form-wrap"><span className="eyebrow">BIENVENIDO DE NUEVO</span><h2>Inicia sesión</h2><p className="muted">Ingresa tus credenciales para continuar.</p>
      <form onSubmit={submit} className="form-stack">
        {notice && <div className="login-notice">{notice}</div>}
        <label>Usuario<input autoComplete="username" value={username} onChange={e => setUsername(e.target.value)} required placeholder="Tu nombre de usuario" /></label>
        <label>Contraseña<input type="password" autoComplete="current-password" value={password} onChange={e => setPassword(e.target.value)} required placeholder="Tu contraseña" /></label>
        {error && <div className="error-banner">{error}. Verifica que Seguridad esté iniciado.</div>}
        <button className="button primary full" disabled={busy}>{busy ? 'Conectando…' : 'Entrar al sistema'}<ArrowUpRight size={17} /></button>
      </form>
      <div className="login-foot"><ShieldCheck size={15} /> Sesión protegida con autenticación JWT</div>
    </div></section>
  </main>;
}

function App() {
  const [session, setSession] = useState(() => {
    try { return JSON.parse(sessionStorage.getItem('ma-session') || 'null'); } catch { return null; }
  });
  const [section, setSection] = useState('Inicio');
  const [patients, setPatients] = useState([]);
  const [appointments, setAppointments] = useState([]);
  const [clinicalAttentions, setClinicalAttentions] = useState([]);
  const [doctors, setDoctors] = useState([]);
  const [specialties, setSpecialties] = useState([]);
  const [consultories, setConsultories] = useState([]);
  const [doctorSchedules, setDoctorSchedules] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [loginNotice, setLoginNotice] = useState('');
  const [showPatientForm, setShowPatientForm] = useState(false);
  const [editingPatient, setEditingPatient] = useState(null);
  const [query, setQuery] = useState('');
  const [toast, setToast] = useState('');
  const [openNav, setOpenNav] = useState('');
  const [profileOpen, setProfileOpen] = useState(false);
  const [profileMode, setProfileMode] = useState('');
  const [patientPage, setPatientPage] = useState(1);
  const [doctorForm,setDoctorForm]=useState(null);
  const [viewingPatient,setViewingPatient]=useState(null);
  const [viewingContacts,setViewingContacts]=useState(null);
  const [prescriptionPatientId,setPrescriptionPatientId]=useState('');

  const loadData = useCallback(async () => {
    setLoading(true); setError('');
    const serviceNames=['Pacientes','Agenda · citas','Agenda · médicos','Agenda · especialidades','Agenda · consultorios','Clínica · atenciones'];
    const results = await Promise.allSettled([
      request(API.patients, '/c?pageNumber=1&pageSize=100', {token:session?.accessToken||session?.token}),
      request(API.agenda, '/api/citas?pageNumber=1&pageSize=100', {token:session?.accessToken||session?.token}),
      request(API.agenda, '/api/medicos?pageNumber=1&pageSize=100', {token:session?.accessToken||session?.token}),
      request(API.agenda, '/api/especialidades?pageNumber=1&pageSize=100', {token:session?.accessToken||session?.token}),
      request(API.agenda, '/api/consultorios?pageNumber=1&pageSize=100', {token:session?.accessToken||session?.token}),
      request(API.clinical, '/api/atenciones', {token:session?.accessToken||session?.token}),
    ]);
    const unauthorized=results.find(result=>result.status==='rejected'&&/\b401\b/.test(result.reason?.message||''));
    if(unauthorized){sessionStorage.removeItem('ma-session');setSession(null);setLoginNotice('Tu sesión venció. Inicia sesión de nuevo para volver a cargar tus citas y expedientes.');setLoading(false);return}
    const [p, a, d, s, c, clinical] = results;
    if (p.status === 'fulfilled') {
      let allPatients=rowsOf(p.value);const total=Number(p.value?.totalRecords??p.value?.total??allPatients.length);const pages=Math.min(50,Math.ceil(total/100));
      if(pages>1){const more=await Promise.allSettled(Array.from({length:pages-1},(_,i)=>request(API.patients,`/c?pageNumber=${i+2}&pageSize=100`,{token:session?.accessToken||session?.token})));allPatients=[...allPatients,...more.filter(x=>x.status==='fulfilled').flatMap(x=>rowsOf(x.value))]}
      setPatients(allPatients);
    }
    if (a.status === 'fulfilled') {
      let allAppointments=rowsOf(a.value);
      const appointmentPages=Math.min(50,Math.ceil(Number(a.value?.totalRecords??a.value?.total??allAppointments.length)/100));
      if(appointmentPages>1){const more=await Promise.allSettled(Array.from({length:appointmentPages-1},(_,i)=>request(API.agenda,`/api/citas?pageNumber=${i+2}&pageSize=100`,{token:session?.accessToken||session?.token})));allAppointments=[...allAppointments,...more.filter(x=>x.status==='fulfilled').flatMap(x=>rowsOf(x.value))]}
      setAppointments(allAppointments);
    }
    if (d.status === 'fulfilled') setDoctors(rowsOf(d.value));
    if (s.status === 'fulfilled') setSpecialties(rowsOf(s.value));
    if (c.status === 'fulfilled') setConsultories(rowsOf(c.value));
    if (clinical.status === 'fulfilled') setClinicalAttentions(rowsOf(clinical.value));
    if (d.status === 'fulfilled') {
      const doctorsList=rowsOf(d.value).filter(doctor=>doctor.activo!==false);
      const scheduleResults=await Promise.allSettled(doctorsList.map(doctor=>request(API.agenda,`/api/horarios-medicos/medico/${doctor.id}?pageNumber=1&pageSize=100&incluirInactivos=true`,{token:session?.accessToken||session?.token})));
      setDoctorSchedules(scheduleResults.flatMap(result=>result.status==='fulfilled'?rowsOf(result.value):[]));
    }
    const failedIndex = results.findIndex(item => item.status === 'rejected');
    if (failedIndex>=0) setError(`No se pudo conectar con ${serviceNames[failedIndex]} (${results[failedIndex].reason.message}). Confirma que el perfil de servicios esté iniciado en Visual Studio.`);
    setLoading(false);
  }, [session]);

  useEffect(() => { if (session) loadData(); }, [session, loadData]);
  useEffect(() => { if (!toast) return; const timer = setTimeout(() => setToast(''), 3000); return () => clearTimeout(timer); }, [toast]);

  const onLogin = value => { sessionStorage.setItem('ma-session', JSON.stringify(value)); setLoginNotice('');setSession(value); };
  const signOut = () => { sessionStorage.removeItem('ma-session'); setSession(null); };
  const normalizedRole=(session?.rol||'').normalize('NFD').replace(/[\u0300-\u036f]/g,'').toLowerCase();
  const isDoctor=normalizedRole==='medico';
  const isReception=normalizedRole==='recepcion'||normalizedRole==='recepcionista';
  const isAdmin=normalizedRole==='administrador';
  const isScheduleSupervisor=normalizedRole==='supervisortecnico';
  const currentDoctor=doctors.find(d=>String(d.id)===String(session?.medicoId));
  const scopedAppointments=isDoctor?(currentDoctor?appointments.filter(a=>String(a.medicoId)===String(currentDoctor.id)):[]):appointments;
  const currentDoctorStatus=currentDoctor?shiftStatus(doctorSchedules.filter(s=>String(s.medicoId)===String(currentDoctor.id)),scopedAppointments):'Fuera de turno';
  const currentDoctorStatusClass=currentDoctorStatus==='En turno'?'online':currentDoctorStatus==='En consulta'?'busy':currentDoctorStatus==='En descanso'?'resting':'off';
  const visiblePatientIds=new Set(scopedAppointments.map(a=>String(a.pacienteId)));
  const patientScope=isDoctor?(currentDoctor?patients.filter(p=>visiblePatientIds.has(String(p.pacienteId))):[]):patients;
  const filteredPatients = useMemo(() => patientScope.filter(patient => `${patient.nombres} ${patient.apellidos} ${patient.numeroDocumento}`.toLowerCase().includes(query.toLowerCase())), [patientScope, query]);
  const pageSize=20;const patientPageCount=Math.max(1,Math.ceil(filteredPatients.length/pageSize));const patientRows=filteredPatients.slice((patientPage-1)*pageSize,patientPage*pageSize);

  if (!session) return <Login onLogin={onLogin} notice={loginNotice}/>;
  const permissions = readPermissions(session.accessToken||session.token||'');
  const canRead = module => isAdmin || permissions.has(`${module}:read`);
  const canDo = (module,action) => isAdmin || permissions.has(`${module}:${action}`);
  const menuGroups = [
    { title:'ESPACIO DE TRABAJO', items:[{label:'Inicio',icon:Activity,module:'dashboard'}] },
    { title:'PACIENTES', items:[{label:'Pacientes',icon:Users,module:'patients'},{label:'Contactos de emergencia',icon:HeartPulse,module:'patients-emergency-contacts'}] },
    { title:'AGENDA', items:[{label:'Citas',icon:CalendarDays,module:'agenda-appointments'},{label:'Médicos',icon:Stethoscope,module:'agenda-doctors'},{label:'Especialidades',icon:ClipboardList,module:'agenda-specialties'},{label:'Consultorios',icon:ClipboardList,module:'agenda-clinics'},{label:'Horarios médicos',icon:Clock3,module:'agenda-schedules'},...(isDoctor?[{label:'Mi horario médico',icon:CalendarDays,module:'agenda-schedules'}]:[])] },
    { title:'ATENCIÓN CLÍNICA', items:[{label:'Atenciones',icon:HeartPulse,module:'clinical-attentions'},{label:'Signos vitales',icon:Activity,module:'clinical-vitals'},{label:'Notas médicas',icon:ClipboardList,module:'clinical-notes'},{label:'Medicamentos',icon:Plus,module:'clinical-medicines'},{label:'Prescripciones',icon:ClipboardList,module:'clinical-prescriptions'}] },
    { title:'SEGURIDAD', items:[{label:'Usuarios',icon:Users,module:'security-users'},{label:'Roles',icon:ShieldCheck,module:'security-roles'},{label:'Auditoría',icon:ClipboardList,module:'security-audit'}] },
  ].map(group=>({...group,items:group.items.filter(item=>canRead(item.module))})).filter(group=>group.items.length>0);
  const upcoming = [...scopedAppointments].filter(a=>new Date(a.fechaHora)>=new Date()&&a.estado!=='Cancelada').sort((x, y) => new Date(x.fechaHora) - new Date(y.fechaHora)).slice(0, 5);
  const doctorFollowUp=isDoctor?clinicalAttentions.filter(a=>String(a.medicoId)===String(session.medicoId)&&a.estado!=='Cerrada').sort((a,b)=>new Date(b.fechaHora)-new Date(a.fechaHora)).slice(0,6):[];

  const groupName=title=>({ 'ESPACIO DE TRABAJO':'Inicio','PACIENTES':'Pacientes','AGENDA':'Agenda','ATENCIÓN CLÍNICA':'Clínica','SEGURIDAD':'Seguridad' }[title]||title);
  const navigate=label=>{setSection(label);setOpenNav('');setProfileOpen(false)};
  const updateProfile=name=>{const next={...session,nombre:name};sessionStorage.setItem('ma-session',JSON.stringify(next));setSession(next)};

  return <div className="app-shell">
    <main className="main-area">
      <header className="topbar horizontal-topbar"><div className="brand top-brand" onClick={()=>navigate('Inicio')}><div className="brand-mark"><HeartPulse size={21}/></div><span>medical<span className="brand-light">appointments</span></span></div><nav className="horizontal-nav">{menuGroups.map(group=><div className="top-nav-group" key={group.title}><button className={`top-nav-trigger ${group.items.some(item=>item.label===section)?'active':''}`} onClick={()=>setOpenNav(openNav===group.title?'':group.title)}>{groupName(group.title)}{group.items.length>1&&<ChevronDown size={13}/>}</button>{openNav===group.title&&<div className="top-nav-menu">{group.items.map(item=><button key={item.label} className={section===item.label?'active':''} onClick={()=>navigate(item.label)}><item.icon size={16}/>{item.label}</button>)}</div>}</div>)}</nav><div className="profile-area">{isDoctor&&currentDoctor&&<div className="topbar-shift"><span className={`doctor-status ${currentDoctorStatusClass}`}>{currentDoctorStatus}</span><button className="button secondary doctor-schedule-link" onClick={()=>navigate('Mi horario médico')}><CalendarDays size={14}/> Mi horario</button><DoctorShiftControl doctor={currentDoctor} schedules={doctorSchedules.filter(s=>String(s.medicoId)===String(currentDoctor.id))} consultories={consultories} token={session.accessToken||session.token} allowed onReload={loadData} onToast={setToast}/></div>}<button className="profile-trigger" onClick={()=>setProfileOpen(!profileOpen)}><div className="top-avatar">{initials(session.nombre)}</div><span className="profile-name">{session.nombre}</span><ChevronDown size={14}/></button>{profileOpen&&<div className="profile-menu"><div className="profile-menu-heading"><strong>{session.nombre}</strong><span>{session.rol}</span></div><button onClick={()=>{setProfileMode('perfil');setProfileOpen(false)}}><UserRound size={16}/> Mi perfil</button><button onClick={()=>{setProfileMode('contrasena');setProfileOpen(false)}}><KeyRound size={16}/> Cambiar contraseña</button><button className="signout-option" onClick={signOut}><X size={16}/> Cerrar sesión</button></div>}</div></header>
      <div className="content-area">
        {error && <div className="error-banner page-error">{error}<button onClick={loadData}>Reintentar</button></div>}
        {section === 'Inicio' && <>
          <div className="page-heading"><div><span className="eyebrow">PANEL GENERAL</span><h1>Buenos días, {session.nombre.split(' ')[0]}</h1><p>Este es el resumen de actividad de tu clínica.</p></div><button className="button secondary" onClick={loadData}>{loading ? 'Actualizando…' : 'Actualizar datos'}<ArrowDownRight size={16} /></button></div>
          <section className="stat-grid">
            <Stat icon={Users} label={isDoctor?'Pacientes asignados':'Pacientes registrados'} value={patientScope.length} change={isDoctor?'Pacientes con citas contigo':'Directorio de pacientes'} color="mint" />
            <Stat icon={CalendarDays} label={isDoctor?'Mis citas':'Citas en agenda'} value={scopedAppointments.length} change="Citas asignadas" color="blue" />
            <Stat icon={Stethoscope} label="Profesionales" value={doctors.length} change={`${specialties.length} especialidades`} color="peach" />
            <Stat icon={Clock3} label="Próxima cita" value={upcoming[0] ? displayTime(upcoming[0].fechaHora) : '—'} change={upcoming[0] ? displayDate(upcoming[0].fechaHora) : 'No hay citas'} color="lavender" />
          </section>
          <section className="dashboard-grid"><div className="panel appointments-panel"><div className="panel-heading"><div><span className="eyebrow">AGENDA</span><h2>{isDoctor?'Mis próximas citas':'Próximas citas'}</h2></div><button className="text-button" onClick={() => setSection('Citas')}>Ver agenda <ArrowUpRight size={15} /></button></div><AppointmentTable rows={upcoming} patients={patientScope} doctors={doctors} onViewPatient={patient=>setViewingPatient(patient)}/></div>
            {isDoctor?<div className="panel quick-panel doctor-followup-panel"><div className="panel-heading"><div><span className="eyebrow">CLÍNICA</span><h2>Atenciones abiertas</h2></div><button className="text-button" onClick={()=>setSection('Atenciones')}>Ver todas <ArrowUpRight size={15}/></button></div>{doctorFollowUp.length?doctorFollowUp.map(attention=>{const patient=patientScope.find(p=>String(p.pacienteId)===String(attention.pacienteId));return <article className="doctor-followup-row" key={attention.id}><div><strong>{patient?`${patient.nombres} ${patient.apellidos}`:'Paciente asignado'}</strong><span>{attention.diagnostico||'En seguimiento'} · {displayDate(attention.fechaHora)}</span></div><button className="text-button" onClick={()=>patient&&setViewingPatient(patient)}>Expediente</button></article>}):<EmptyState icon={HeartPulse} text="No tienes atenciones abiertas para seguimiento."/>}<button className="quick-link" onClick={()=>setSection('Atenciones')}><span className="quick-icon blue"><Plus size={18}/></span><span><strong>Iniciar atención</strong><small>Selecciona una cita de un paciente que llegó</small></span><ArrowUpRight size={16}/></button></div>:<div className="panel quick-panel"><div className="panel-heading"><div><span className="eyebrow">ACCESOS RÁPIDOS</span><h2>¿Qué necesitas hacer?</h2></div></div><button className="quick-link" onClick={() => { setSection('Pacientes'); setShowPatientForm(true); }}><span className="quick-icon mint"><Plus size={18} /></span><span><strong>Registrar paciente</strong><small>Crear un nuevo expediente</small></span><ArrowUpRight size={16} /></button><button className="quick-link" onClick={() => setSection('Citas')}><span className="quick-icon blue"><CalendarDays size={18} /></span><span><strong>Revisar agenda</strong><small>Consultar las citas programadas</small></span><ArrowUpRight size={16} /></button><button className="quick-link" onClick={() => setSection('Médicos')}><span className="quick-icon peach"><Stethoscope size={18} /></span><span><strong>Ver equipo médico</strong><small>Profesionales y especialidades</small></span><ArrowUpRight size={16} /></button><div className="integration-note"><span className="integration-dot" /><span>Servicios: <b>Seguridad</b>, <b>Pacientes</b>, <b>Agenda</b> y <b>Clínica</b></span></div></div>}</section>
        </>}
        {section === 'Pacientes' && <><div className="page-heading"><div><span className="eyebrow">DIRECTORIO CLÍNICO</span><h1>Pacientes</h1><p>{isDoctor?`Pacientes asignados a ${currentDoctor?.nombre||'tu cuenta médica'} según sus citas.`:'Consulta expedientes, datos de contacto e historial clínico.'}</p></div>{canDo('patients','create')&&<button className="button primary" onClick={() => {setEditingPatient(null);setShowPatientForm(true);}}><Plus size={17} /> Nuevo paciente</button>}</div><div className="panel table-panel"><div className="table-toolbar"><div className="search-box"><Search size={16} /><input value={query} onChange={e => {setQuery(e.target.value);setPatientPage(1)}} placeholder="Buscar por nombre o documento" /></div><span className="result-count">{filteredPatients.length} pacientes · {pageSize} por página</span></div><div className="patient-card-grid">{patientRows.map(person=><article className="patient-record-card" key={person.pacienteId}><div className="patient-card-top"><div className="patient-avatar">{initials(`${person.nombres} ${person.apellidos}`)}</div><span className={`badge ${person.activo?'programada':'cancelada'}`}>{person.activo?'Activo':'Inactivo'}</span></div><strong>{person.nombres} {person.apellidos}</strong><small>{person.codigoPaciente||'Expediente clínico'}</small><div className="patient-record-meta"><span><b>Teléfono:</b> {person.telefono||'Sin teléfono'}</span><span><b>Correo:</b> {person.email||'Sin correo'}</span><span><b>Documento:</b> {person.tipoDocumento}: {person.numeroDocumento}</span></div><div className="patient-card-actions"><button className="text-button" onClick={()=>setViewingPatient(person)}>Ver expediente</button>{canRead('patients-emergency-contacts')&&<button className="text-button" onClick={()=>setViewingContacts(person)}>Contactos</button>}{canDo('patients','edit')&&<button className="text-button" onClick={()=>{setEditingPatient(person);setShowPatientForm(true)}}>Editar datos</button>}</div></article>)}</div>{filteredPatients.length>pageSize&&<div className="pagination-controls"><button className="button secondary" disabled={patientPage<=1} onClick={()=>setPatientPage(patientPage-1)}>Anterior</button><span>Página {patientPage} de {patientPageCount}</span><button className="button secondary" disabled={patientPage>=patientPageCount} onClick={()=>setPatientPage(patientPage+1)}>Siguiente</button></div>}{filteredPatients.length===0&&<div className="empty-state">{isDoctor?'No hay pacientes con citas asignadas.':'No se encontraron pacientes.'}</div>}</div></>}
        {section === 'Contactos de emergencia' && <EmergencyContacts patients={patientScope} token={session.accessToken||session.token} canCreate={canDo('patients-emergency-contacts','create')} canEdit={canDo('patients-emergency-contacts','edit')} onToast={setToast}/>}
        {section === 'Citas' && <AppointmentWorkspace appointments={scopedAppointments} patients={patientScope} doctors={doctors} specialties={specialties} consultories={consultories} session={session} canCreate={isAdmin||isReception} canEdit={isAdmin||isDoctor} isDoctor={isDoctor} onReload={loadData} onToast={setToast}/>}
        {section === 'Médicos' && <><div className="page-heading"><div><span className="eyebrow">AGENDA · PROFESIONALES</span><h1>Médicos</h1><p>El estado refleja el turno actual y las citas. Los turnos se asignan desde el cronograma mensual.</p></div><div className="page-heading-actions">{isDoctor&&<button className="button secondary" onClick={()=>navigate('Mi horario médico')}><CalendarDays size={16}/> Mi horario médico</button>}{isAdmin&&<button className="button primary" onClick={()=>setDoctorForm({})}><Plus size={16}/> Nuevo médico</button>}</div></div><div className="doctor-grid">{doctors.map(doctor => {const ownAccount=isDoctor&&String(currentDoctor?.id)===String(doctor.id);const status=doctor.activo?shiftStatus(doctorSchedules.filter(s=>String(s.medicoId)===String(doctor.id)),appointments.filter(a=>String(a.medicoId)===String(doctor.id))):'Inactivo';return <div className="doctor-card" key={doctor.id}><div className="doctor-avatar">{initials(`${doctor.nombre} ${doctor.apellido}`)}</div><div className="doctor-details"><strong>Dr. {doctor.nombre} {doctor.apellido}</strong><span>{doctor.especialidad?.nombre || specialties.find(specialty => specialty.id === doctor.especialidadId)?.nombre || 'Especialidad no asignada'}</span><small>{doctor.correoElectronico || doctor.telefono || '—'}</small></div><span className={`doctor-status ${status==='En turno'?'online':status==='En consulta'?'busy':status==='En descanso'?'resting':''}`}>{status}</span>{(isAdmin||ownAccount)&&doctor.activo&&<DoctorShiftControl doctor={doctor} schedules={doctorSchedules.filter(s=>String(s.medicoId)===String(doctor.id))} consultories={consultories} token={session.accessToken||session.token} allowed onReload={loadData} onToast={setToast}/ >}{isAdmin&&<button className="icon-button" onClick={()=>setDoctorForm(doctor)} title="Editar médico"><Pencil size={15}/></button>}</div>})}</div>{doctors.length===0&&<EmptyState icon={Stethoscope} text="No hay profesionales para mostrar."/>}</>}
        {['Especialidades','Consultorios','Horarios médicos'].includes(section)&&<AgendaCatalog section={section} token={session.token||session.accessToken} session={session} doctors={doctors} patients={patientScope} consultories={consultories} appointments={scopedAppointments} onToast={setToast} onReload={loadData}/>}
        {section==='Mi horario médico'&&<MonthlyScheduleBoard token={session.accessToken||session.token} session={session} doctors={doctors} consultories={consultories} onToast={setToast} onReload={loadData}/>}
        {section==='Notas médicas'&&<ClinicalNotesBoard token={session.token||session.accessToken} session={session} patients={patientScope} doctors={doctors} consultories={consultories}/>}
        {['Medicamentos','Prescripciones'].includes(section)&&<ClinicalRecordsBoard section={section} token={session.token||session.accessToken} session={session} patients={patientScope} doctors={doctors} consultories={consultories} initialPatientId={prescriptionPatientId} onInitialPrescriptionConsumed={()=>setPrescriptionPatientId('')}/>}
        {['Atenciones','Signos vitales'].includes(section)&&<ClinicalOverviewBoard section={section} token={session.accessToken||session.token} session={session} patients={patientScope} doctors={doctors} consultories={consultories}/>}
        {['Usuarios','Roles','Auditoría'].includes(section) && <Maintenance section={section} token={session.token || session.accessToken} session={session} doctors={doctors} consultories={consultories} patients={patientScope} appointments={scopedAppointments} onToast={setToast} onReload={loadData} />}
        <footer className="page-footer"><span>MedicalAppointments · Panel clínico</span><span><span className="status-led" /> Sistema operativo</span></footer>
      </div>
    </main>
    {showPatientForm && <PatientModal patient={editingPatient} onClose={() => setShowPatientForm(false)} onSaved={() => { setShowPatientForm(false); setToast(editingPatient?'Paciente actualizado correctamente':'Paciente registrado correctamente'); loadData(); }} onError={setToast} />}
    {viewingPatient&&<PatientRecordModal patient={viewingPatient} token={session.accessToken||session.token} canCreateContacts={canDo('patients-emergency-contacts','create')} canReadClinical={canRead('clinical-attentions')} canReadPrescriptions={canRead('clinical-prescriptions')} canCreatePrescription={canDo('clinical-prescriptions','create')} onCreatePrescription={()=>{setPrescriptionPatientId(String(viewingPatient.pacienteId));setViewingPatient(null);setSection('Prescripciones')}} onClose={()=>setViewingPatient(null)} onToast={setToast}/>}
    {viewingContacts&&<EmergencyContacts patients={[viewingContacts]} patient={viewingContacts} patientId={viewingContacts.pacienteId} token={session.accessToken||session.token} canCreate={canDo('patients-emergency-contacts','create')} canEdit={canDo('patients-emergency-contacts','edit')} onToast={setToast} onClose={()=>setViewingContacts(null)}/>}
    {doctorForm&&<DoctorModal doctor={doctorForm.id?doctorForm:null} specialties={specialties} token={session.accessToken||session.token} onClose={()=>setDoctorForm(null)} onSaved={()=>{setDoctorForm(null);setToast('Médico guardado');loadData()}}/>}
    {profileMode&&<ProfileModal mode={profileMode} session={session} token={session.accessToken||session.token} onClose={()=>setProfileMode('')} onProfileSaved={updateProfile} onToast={setToast}/>}
    {toast && <div className="toast"><span className="toast-check">✓</span>{toast}<button onClick={() => setToast('')}><X size={15} /></button></div>}
  </div>;
}

function Stat({ icon: Icon, label, value, change, color }) {
  return <article className="stat-card"><div className={`stat-icon ${color}`}><Icon size={18} /></div><div className="stat-label">{label}</div><strong className="stat-value">{value}</strong><div className="stat-change">{change}</div></article>;
}

function AppointmentTable({ rows, patients=[], doctors=[], expanded = false, onViewPatient }) {
  if (!rows.length) return <EmptyState icon={ClipboardList} text="No hay citas registradas." />;
  return <div className="table-wrap"><table><thead><tr><th>Paciente</th><th>Profesional</th><th>Fecha y hora</th><th>Consultorio</th><th>Estado</th>{expanded && <th>Motivo</th>}{onViewPatient&&<th>Acción</th>}</tr></thead><tbody>{rows.map((item, index) => {const patient=patients.find(p=>String(p.pacienteId)===String(item.pacienteId));const doctor=item.medico||doctors.find(d=>String(d.id)===String(item.medicoId));return <tr key={item.id || index}><td><div className="cell-main">{patient?`${patient.nombres} ${patient.apellidos}`:'Paciente no disponible'}</div></td><td><div className="cell-main">{doctor ? `Dr. ${doctor.nombre} ${doctor.apellido}` : 'Médico no asignado'}</div><div className="cell-sub">{doctor?.especialidad?.nombre || 'Equipo médico'}</div></td><td><div className="cell-main">{displayDate(item.fechaHora)}</div><div className="cell-sub">{displayTime(item.fechaHora)}</div></td><td>{item.consultorio?.nombre||'Por confirmar'}</td><td><span className={`badge ${String(item.estado).toLowerCase().replaceAll(' ', '-')}`}>{item.estado}</span></td>{expanded && <td>{item.motivoConsulta || '—'}</td>}{onViewPatient&&<td><button className="text-button" disabled={!patient} onClick={()=>patient&&onViewPatient(patient)}>Ver expediente</button></td>}</tr>})}</tbody></table></div>;
}

function PatientTable({ rows, onEdit, onToggle }) {
  if (!rows.length) return <EmptyState icon={Users} text="No se encontraron pacientes." />;
  return <div className="table-wrap"><table><thead><tr><th>Paciente</th><th>Documento</th><th>Teléfono</th><th>Correo</th><th>Estado</th><th>Acciones</th></tr></thead><tbody>{rows.map(person => <tr key={person.pacienteId}><td><div className="patient-cell"><div className="patient-avatar">{initials(`${person.nombres} ${person.apellidos}`)}</div><div><div className="cell-main">{person.nombres} {person.apellidos}</div><div className="cell-sub">{person.codigoPaciente || 'Expediente clínico'}</div></div></div></td><td>{person.tipoDocumento}: {person.numeroDocumento}</td><td>{person.telefono || '—'}</td><td>{person.email || '—'}</td><td><span className={`badge ${person.activo ? 'programada' : 'cancelada'}`}>{person.activo ? 'Activo' : 'Inactivo'}</span></td><td><button className="text-button" onClick={()=>onEdit(person)}>Editar</button>{person.activo&&<button className="text-button" onClick={()=>onToggle(person)}>Desactivar</button>}</td></tr>)}</tbody></table></div>;
}

function EmptyState({ icon: Icon, text }) {
  return <div className="empty-state"><div className="empty-icon"><Icon size={22} /></div><span>{text}</span></div>;
}

function ProfileModal({mode,session,token,onClose,onProfileSaved,onToast}) {
 const isProfile=mode==='perfil';const [error,setError]=useState('');const [busy,setBusy]=useState(false);
 const submit=async event=>{event.preventDefault();setError('');const values=Object.fromEntries(new FormData(event.currentTarget));if(!isProfile&&values.contrasenaNueva!==values.confirmacion){setError('La confirmación no coincide con la contraseña nueva.');return;}if(!isProfile&&values.contrasenaNueva.length<12){setError('La contraseña nueva debe tener al menos 12 caracteres.');return;}setBusy(true);try{const response=await fetch(`/backend/security/api/cuenta/${isProfile?'perfil':'contrasena'}`,{method:'PATCH',headers:{'Content-Type':'application/json',...(token?{Authorization:`Bearer ${token}`}:{})},body:JSON.stringify(isProfile?{nombre:values.nombre}:{contrasenaActual:values.contrasenaActual,contrasenaNueva:values.contrasenaNueva})});if(!response.ok){let message=`${response.status} ${response.statusText}`;try{const body=await response.json();message=body.message||body.detail||message}catch{}throw new Error(message)}if(isProfile)onProfileSaved(values.nombre.trim());onToast(isProfile?'Nombre actualizado':'Contraseña cambiada');onClose()}catch(e){setError(e.message)}finally{setBusy(false)}};
 return <div className="modal-backdrop" onMouseDown={e=>e.target===e.currentTarget&&onClose()}><section className="modal profile-modal"><div className="modal-header"><div><span className="eyebrow">MI CUENTA</span><h2>{isProfile?'Editar nombre visible':'Cambiar contraseña'}</h2></div><button className="icon-button" onClick={onClose}><X size={18}/></button></div><form className="patient-form profile-form" onSubmit={submit}>{isProfile?<label className="span-two">Nombre que se muestra en el sistema<input name="nombre" maxLength={150} minLength={2} defaultValue={session.nombre||''} required/></label>:<><label className="span-two">Contraseña actual<input type="password" name="contrasenaActual" autoComplete="current-password" required/></label><label className="span-two">Contraseña nueva<input type="password" name="contrasenaNueva" autoComplete="new-password" minLength={12} required/><small>Usa al menos 12 caracteres.</small></label><label className="span-two">Confirmar contraseña nueva<input type="password" name="confirmacion" autoComplete="new-password" minLength={12} required/></label></>}{error&&<div className="error-banner span-two">{error}</div>}<div className="modal-actions"><button type="button" className="button secondary" onClick={onClose}>Cancelar</button><button className="button primary" disabled={busy}>{busy?'Guardando…':'Guardar cambios'}</button></div></form></section></div>;
}

function DoctorModal({doctor,specialties,token,onClose,onSaved}){
 const [error,setError]=useState('');const [busy,setBusy]=useState(false);
 const submit=async event=>{event.preventDefault();setError('');setBusy(true);const raw=Object.fromEntries(new FormData(event.currentTarget));const body={...raw,especialidadId:Number(raw.especialidadId)};try{const r=await fetch(`/backend/agenda/api/medicos${doctor?`/${doctor.id}`:''}`,{method:doctor?'PUT':'POST',headers:{'Content-Type':'application/json',...(token?{Authorization:`Bearer ${token}`}:{})},body:JSON.stringify(doctor?{id:doctor.id,...body}:body)});if(!r.ok){let message=`${r.status} ${r.statusText}`;try{const response=await r.json();message=response.message||response.detail||message}catch{}throw new Error(message)}onSaved()}catch(e){setError(e.message)}finally{setBusy(false)}};
 return <div className="modal-backdrop" onMouseDown={e=>e.target===e.currentTarget&&onClose()}><section className="modal"><div className="modal-header"><div><span className="eyebrow">EQUIPO MÉDICO</span><h2>{doctor?'Editar médico':'Registrar médico'}</h2></div><button className="icon-button" onClick={onClose}><X size={18}/></button></div><form className="patient-form" onSubmit={submit}><label>Nombre<input name="nombre" maxLength={100} defaultValue={doctor?.nombre||''} required/></label><label>Apellido<input name="apellido" maxLength={100} defaultValue={doctor?.apellido||''} required/></label><label>Código médico<input name="codigoMedico" maxLength={50} defaultValue={doctor?.codigoMedico||doctor?.codigomedico||''} required/></label><label>Especialidad<select name="especialidadId" defaultValue={doctor?.especialidadId||''} required><option value="">Selecciona especialidad</option>{specialties.filter(s=>s.activo!==false).map(s=><option key={s.id} value={s.id}>{s.nombre}</option>)}</select></label><label>Tipo de documento<select name="tipoDocumento" defaultValue={doctor?.tipoDocumento||'DUI'}><option value="DUI">DUI</option><option value="Pasaporte">Pasaporte</option><option value="Otro">Otro</option></select></label><label>Número de documento<input name="numeroDocumento" maxLength={50} defaultValue={doctor?.numeroDocumento||doctor?.numeroDocumento||''}/></label><label>Teléfono<input name="telefono" type="tel" maxLength={30} pattern="[0-9+() -]{7,30}" defaultValue={doctor?.telefono||''}/></label><label>Correo<input name="correoElectronico" type="email" maxLength={150} defaultValue={doctor?.correoElectronico||''}/></label>{error&&<div className="error-banner span-two">{error}</div>}<div className="modal-actions"><button type="button" className="button secondary" onClick={onClose}>Cancelar</button><button className="button primary" disabled={busy}>{busy?'Guardando…':'Guardar médico'}</button></div></form></section></div>
}

function PatientModal({ patient, onClose, onSaved, onError }) {
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');
  const submit = async event => {
    event.preventDefault(); setBusy(true); setError('');
    const values = Object.fromEntries(new FormData(event.currentTarget));
    const body = { ...(patient||{}), ...values, pacienteId:patient?.pacienteId||0, codigoPaciente:patient?.codigoPaciente||null, fechaNacimiento: values.fechaNacimiento, activo: patient?.activo??true };
    try { await request(API.patients, patient?`/c/${patient.pacienteId}`:'/c/Create', { method: patient?'PUT':'POST', body: JSON.stringify(body) }); onSaved(); }
    catch (exception) { setError(exception.message); onError(exception.message); }
    finally { setBusy(false); }
  };
  const today=new Date().toISOString().slice(0,10);
  return <div className="modal-backdrop" onMouseDown={event => event.target === event.currentTarget && onClose()}><section className="modal"><div className="modal-header"><div><span className="eyebrow">{patient?'ACTUALIZAR REGISTRO':'NUEVO REGISTRO'}</span><h2>{patient?'Editar paciente':'Registrar paciente'}</h2></div><button className="icon-button" onClick={onClose}><X size={18} /></button></div><form className="patient-form" onSubmit={submit}><label>Nombres<input name="nombres" maxLength={100} defaultValue={patient?.nombres||''} required /></label><label>Apellidos<input name="apellidos" maxLength={100} defaultValue={patient?.apellidos||''} required /></label><label>Tipo de documento<select name="tipoDocumento" defaultValue={patient?.tipoDocumento||'DUI'} required><option value="DUI">DUI</option><option value="Pasaporte">Pasaporte</option><option value="Otro">Otro</option></select></label><label>Número de documento<input name="numeroDocumento" maxLength={50} pattern="[A-Za-z0-9.-]{5,50}" title="Usa entre 5 y 50 letras, números, punto o guion." defaultValue={patient?.numeroDocumento||''} required /></label><label>Fecha de nacimiento<input name="fechaNacimiento" type="date" max={today} defaultValue={patient?.fechaNacimiento?.slice(0,10)||''} required /></label><label>Sexo<select name="sexo" defaultValue={patient?.sexo||'F'} required><option value="F">Femenino</option><option value="M">Masculino</option><option value="O">Otro</option></select></label><label>Teléfono<input name="telefono" type="tel" maxLength={30} pattern="[0-9+() -]{7,30}" defaultValue={patient?.telefono||''} /></label><label>Estado civil<input name="estadoCivil" maxLength={30} defaultValue={patient?.estadoCivil||''} /></label><label className="span-two">Correo electrónico<input name="email" type="email" maxLength={150} defaultValue={patient?.email||''} /></label><label className="span-two">Dirección<input name="direccion" maxLength={300} defaultValue={patient?.direccion||''} /></label>{error && <div className="error-banner span-two">{error}</div>}<div className="modal-actions"><button type="button" className="button secondary" onClick={onClose}>Cancelar</button><button className="button primary" disabled={busy}>{busy ? 'Guardando…' : 'Guardar paciente'}<ArrowUpRight size={16} /></button></div></form></section></div>;
}

export default App;
