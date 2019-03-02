import * as React from 'react';
import * as styles from './TestTools.scss';
import { IItemBoxProps } from '../itemBox/itemBox';
import { ListBox } from '../listBox/listBox';

export interface ITestToolsState {
    items?: IItemBoxProps[];

}

export interface ITestToolsProps {
    favouritedItems?: IItemBoxProps[];
}


export class TestTools extends React.Component<ITestToolsProps, ITestToolsState> {
    public static readonly ID = "FAVOURITES_LIST_BOX_ID";

    constructor(props: ITestToolsProps) {
        super(props);
        this.state = { items: [] };
    }

    render() {
        return (
            <div className={styles.container}>
                <div className={styles.title}>
                    Favourites
                </div>
                <ListBox
                    id={TestTools.ID}
                    items={this.state.items}
                    noItemsMessage={"No favourites added yet."}
                    favouritesCanBeRemoved
                />
            </div>
        );
    }
}
