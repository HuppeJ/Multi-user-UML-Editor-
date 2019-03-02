import * as React from 'react';
import * as styles from './itemBox.scss';

export interface IItemBoxOwnProps {
    title?: string;
    description?: string;
    favouritesCanBeAdded?: boolean;
    favouritesCanBeRemoved?: boolean;
}

export interface IItemBoxStateProps {
    favouritedItems?: IItemBoxOwnProps[];
}

export interface IItemBoxDispatchProps {
    addFavourite?: (item: IItemBoxOwnProps) => void;
    removeFavourite?: (item: IItemBoxOwnProps) => void;
}

export interface IItemBoxProps extends IItemBoxOwnProps, IItemBoxStateProps, IItemBoxDispatchProps {}

export interface IItemBoxState {
    isFavourited?: boolean;
}

export class ItemBox extends React.Component<IItemBoxProps, IItemBoxState> {
    static defaultProps: Partial<IItemBoxProps> = {
        title: '',
        description: '',
        favouritesCanBeAdded: false,
        favouritesCanBeRemoved: false
    };

    constructor(props: IItemBoxProps) {
        super(props);
        this.state = { isFavourited: this.isFavourited() };
    }

    componentWillReceiveProps(newProps: IItemBoxProps) {
        const isFavourited: boolean = newProps.favouritedItems.find(item => item.title === this.props.title) != null;
        this.setState({ isFavourited });
    }

    private getDecodedDescriptions(): string {
        let node: HTMLElement = document.createElement('div');
        node.innerHTML = this.props.description;
        return node.childNodes.length === 0 ? "" : node.childNodes[0].nodeValue;
    }

    private onStarClick(): void {
        if (this.state.isFavourited && this.props.favouritesCanBeRemoved) {
            this.props.removeFavourite(this.props);
        } else if (!this.state.isFavourited && this.props.favouritesCanBeAdded) {
            this.props.addFavourite(this.props);
        }
    }

    private isFavourited(): boolean {
        return this.props.favouritedItems.find(item => item.title === this.props.title) != null;
    }

    render() {
        const starStyle = this.state.isFavourited ? styles.starSelected : styles.starUnselected;

        return (
            <div className={styles.container}>
                <label className={`${styles.star} ${starStyle}`} onClick={() => this.onStarClick()}>
                    ★
                </label>
                <div className={styles.title}>
                    {this.props.title}
                </div>
                <div className={styles.description} dangerouslySetInnerHTML={{ __html: this.getDecodedDescriptions() }} /> 
            </div>
        );
    }
}
