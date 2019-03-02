import * as Redux from 'redux';
import { WasteLookupAppReducer } from '../wasteLookupApp/wasteLookupAppReducers';

export const RootReducers = Redux.combineReducers({
    WasteLookupApp: WasteLookupAppReducer
});
