import * as React from 'react';
import searchIcon from './../../img/searchIcon.png';
import * as styles from './searchBar.scss';

export interface ISearchBarProps {
    onSearch?: (inputText: string) => void;
    onEmptyInput?: () => void;
    isSearching?: boolean;
    placeholder?: string;
}

export interface ISearchBarState {
    value: string;
}

export class SearchBar extends React.Component<ISearchBarProps, ISearchBarState> {
    static defaultProps: Partial<ISearchBarProps> = {
        isSearching: false,
        placeholder: ''
    };

    constructor(props: ISearchBarProps) {
        super(props);
        this.state = { value: '' };
    }

    private onChange(e: React.FormEvent<HTMLInputElement>) {
        const input: string = e.currentTarget.value;
        this.setState({ value: input });
        if (input === "") {
            this.props.onEmptyInput();
        }
    }

    private onKeyDown(e: React.KeyboardEvent) {
        if (e.key == 'Enter') {
            this.onSearch();
        }
    }

    private onSearch() {
        if (!this.props.isSearching) {
            this.props.onSearch(this.state.value);
        }
    }

    render() {
        return (
            <div className={styles.container}>
                <input
                    type="text"
                    className={styles.input}
                    onChange={(e) => this.onChange(e)}
                    onKeyDown={(e) => this.onKeyDown(e)}
                    placeholder={this.props.placeholder}
                />
                <button className={styles.button} onClick={() => this.onSearch()}>
                    <img className={styles.searchIcon} src={searchIcon} />
                </button>
            </div>
        );
    }
}
