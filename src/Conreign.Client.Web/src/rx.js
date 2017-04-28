import Rx from 'rxjs';
import { mapIf, doIf } from './util';

Rx.Observable.prototype.mapIf = mapIf;
Rx.Observable.prototype.doIf = doIf;

export * from 'rxjs';
export { default } from 'rxjs';
