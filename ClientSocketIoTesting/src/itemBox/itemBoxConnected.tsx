import * as React from 'react';
import { connect } from 'react-redux';
import { IItemBoxStateProps, IItemBoxDispatchProps, IItemBoxOwnProps, IItemBoxProps, ItemBox } from './itemBox';
import { addFavourite, removeFavourite } from '../wasteLookupApp/wasteLookupAppActions';
import { IDispatch } from '../utils/ReduxInterfaces';

const mapStateToProps = (state: any): IItemBoxStateProps => {
    return {
        favouritedItems: state.WasteLookupApp.favouritedItems ? state.WasteLookupApp.favouritedItems : [],
    };
};

const mapDispatchToProps = (dispatch: IDispatch): IItemBoxDispatchProps => ({
    addFavourite: (item: IItemBoxOwnProps) => dispatch(addFavourite(item)),
    removeFavourite: (item: IItemBoxOwnProps) => dispatch(removeFavourite(item))
});

export const ItemBoxConnected: React.ComponentClass<IItemBoxProps> = connect(mapStateToProps, mapDispatchToProps)(ItemBox);
