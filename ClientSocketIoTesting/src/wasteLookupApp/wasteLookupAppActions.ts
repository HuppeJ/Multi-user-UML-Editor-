import { IItemBoxOwnProps } from "../itemBox/itemBox";

export const WasteLookupAppActions = {
    add: 'ADD_FAVOURITE',
    remove: 'REMOVE_FAVOURITE',
};

export interface IWasteLookupAppPayload {
    item: IItemBoxOwnProps;
}

export const addFavourite = (item: IItemBoxOwnProps) => ({
    type: WasteLookupAppActions.add,
    payload: { item }
});

export const removeFavourite = (item: IItemBoxOwnProps) => ({
    type: WasteLookupAppActions.remove,
    payload: { item }
});