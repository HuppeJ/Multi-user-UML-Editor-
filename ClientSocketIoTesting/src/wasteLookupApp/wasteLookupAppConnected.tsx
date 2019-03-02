import * as React from 'react';
import { connect } from 'react-redux';
import { IAppState } from '../app/appState';
import { IWasteLookupAppStateProps, IWasteLookupAppProps, WasteLookupApp } from './wasteLookupApp';

const mapStateToProps = (state: IAppState): IWasteLookupAppStateProps => {
    return {
        favouritedItems: state.favouritedItems ? state.favouritedItems : [],
    };
};

export const WasteLookupAppConnected: React.ComponentClass<IWasteLookupAppProps> = connect(mapStateToProps)(WasteLookupApp);