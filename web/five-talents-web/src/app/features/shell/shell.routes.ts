import { Routes } from '@angular/router';
export const SHELL_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./shell.component').then(m => m.ShellComponent),
    children: [
      { path: 'dashboard', loadComponent: () => import('../dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { path: 'members', loadComponent: () => import('../members/members.component').then(m => m.MembersComponent) },
      { path: 'members/new', loadComponent: () => import('../members/member-form/member-form.component').then(m => m.MemberFormComponent) },
      { path: 'members/:id', loadComponent: () => import('../members/member-detail/member-detail.component').then(m => m.MemberDetailComponent) },
      { path: 'members/:id/edit', loadComponent: () => import('../members/member-form/member-form.component').then(m => m.MemberFormComponent) },
      { path: 'groups', loadComponent: () => import('../groups/groups.component').then(m => m.GroupsComponent) },
      { path: 'families', loadComponent: () => import('../families/families.component').then(m => m.FamiliesComponent) },
      { path: 'families/:id', loadComponent: () => import('../families/family-detail/family-detail.component').then(m => m.FamilyDetailComponent) },
      { path: 'attendance', loadComponent: () => import('../attendance/attendance.component').then(m => m.AttendanceComponent) },
      { path: 'giving', loadComponent: () => import('../giving/giving.component').then(m => m.GivingComponent) },
      { path: 'events', loadComponent: () => import('../events/events.component').then(m => m.EventsComponent) },
      { path: 'sermons', loadComponent: () => import('../sermons/sermons.component').then(m => m.SermonsComponent) },
      { path: 'volunteers', loadComponent: () => import('../volunteers/volunteers.component').then(m => m.VolunteersComponent) },
      { path: 'communication', loadComponent: () => import('../communication/communication.component').then(m => m.CommunicationComponent) },
      { path: 'organizations', loadComponent: () => import('../organizations/organizations.component').then(m => m.OrganizationsComponent) },
      { path: 'settings', loadComponent: () => import('../settings/settings.component').then(m => m.SettingsComponent) },
      { path: 'profile', loadComponent: () => import('../profile/profile.component').then(m => m.ProfileComponent) },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  }
];
