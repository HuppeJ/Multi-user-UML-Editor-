import { IWasteLookupAppPayload, WasteLookupAppActions } from './wasteLookupAppActions';
import { IItemBoxOwnProps } from '../itemBox/itemBox';
import { IReduxAction } from '../utils/ReduxInterfaces';

export interface IWasteLookupAppState {
    favouritedItems?: IItemBoxOwnProps[];
}

export const wasteLookupAppState: IWasteLookupAppState = {favouritedItems: []};

export const WasteLookupAppReducer = (state: IWasteLookupAppState = wasteLookupAppState, action: IReduxAction<IWasteLookupAppPayload>) => {
    switch (action.type) {
        case WasteLookupAppActions.add: {
            return {
                ...state,
                favouritedItems: [...state.favouritedItems, action.payload.item]
            }
        }
        case WasteLookupAppActions.remove: {
            return {
                ...state,
                favouritedItems: state.favouritedItems.filter(item => item.title !== action.payload.item.title)
            }
        }
        default: {
            return state
        }
    }
}