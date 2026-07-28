import { Routes } from '@angular/router';
import { ClassInstanceDetailPage } from './pages/class-instances/class-instance-detail';
import { ClassInstancesList } from './pages/class-instances/class-instances-list';
import { ClassTypeDetailPage } from './pages/class-types/class-type-detail';
import { ClassTypesList } from './pages/class-types/class-types-list';
import { ConfigContactsPage } from './pages/class-types/config-contacs';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'instances' },
  { path: 'class-types', component: ClassTypesList },
  { path: 'class-types/:id', component: ClassTypeDetailPage },
  { path: 'class-types/:typeId/config-contacts', component: ConfigContactsPage },
  { path: 'instances', component: ClassInstancesList },
  { path: 'instances/:id', component: ClassInstanceDetailPage },
  { path: '**', redirectTo: 'instances' }
];
