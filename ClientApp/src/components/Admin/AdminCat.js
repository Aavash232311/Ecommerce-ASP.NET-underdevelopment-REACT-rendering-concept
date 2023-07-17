import React, { Component } from "react";
import { AdminNav } from "./AdminNav";
import { Services } from "../../utils/services";
import "../../style/admin_cat.css";
import 'bootstrap/dist/css/bootstrap.css';


export class Category extends Component {
    constructor(props) {
        super(props);
        this.utils = new Services();
        this.componentDidMount = this.componentDidMount.bind(this);
        this.getInitialLoad = this.getInitialLoad.bind(this);
        this.scrollPagination = this.scrollPagination.bind(this);
        this.getChild = this.getChild.bind(this);
        this.search = this.search.bind(this);
        this.select = this.select.bind(this);
        this.utils = new Services();
        this.state = {
            initialLoad: {},
            children: {},
            parent: [],
            value: null
        }
    }


    getInitialLoad() {
        fetch(this.utils.getServer() + "/admin/getMainCategory?parent=null", {
            headers: {
                'Authorization': 'Bearer ' + this.utils.token(),
                "Content-Type": "application/json",
            },
        }).then(x => x.json()).then((repsonse) => {
            // parnet header tag 
            this.setState({ initialLoad: repsonse.value });
        })
    }

    componentDidMount() {
        this.getInitialLoad();
    }

    scrollPagination(ev, parent) {
        let last = this.state.initialLoad[this.state.initialLoad.length - 1].createdAt;
        const parentCopy = parent;
        if (parent === 1) {
            last = this.state.parent[this.state.parent.length - 1]; // parent node
            parent = last.id;
            last = this.state.children[this.state.children.length - 1].createdAt;
        }
        if (last !== undefined) {
            const server = this.utils.getServer() + "/admin/first_in_cat?date=" + last + "&parent=" + parent;
            fetch(server, {
                headers: {
                    'Authorization': 'Bearer ' + this.utils.token(),
                    "Content-Type": "application/json",
                },
            }).then(x => x.json()).then((response) => {
                if (response.value !== undefined) {
                    if (response.value.length === 0) {
                        alert("You got caught");
                        return;
                    }
                    for (let i = 0; i < response.value.length; i++) {
                        if (parentCopy === 1) {
                            this.setState(prevState => ({
                                children: [...prevState.children, response.value[i]]
                            }));
                        } else {
                            this.setState(prevState => ({
                                initialLoad: [...prevState.initialLoad, response.value[i]]
                            }));
                        }
                    }
                }
            })
        }
    }

    getChild(parent, type) {
        const parentId = parent.id;
        fetch(this.utils.getServer() + "/admin/getMainCategory?parent=" + parentId, {
            headers: {
                'Authorization': 'Bearer ' + this.utils.token(),
                "Content-Type": "application/json",
            },
        }).then(x => x.json()).then((response) => {
            if (response.length > 0) {
                this.setState({ children: response }, () => { });
                const checkForParent = this.state.parent.filter(x => x.id === parentId);
                if (checkForParent.length === 0) {
                    if (type === "p") {
                        this.setState((x) => ({
                            parent: []
                        }))
                    }
                    this.setState((x) => (
                        {
                            parent: [...x.parent, parent]
                        }
                    ))

                }
            }
        })
    }

    search(ev, type) {
        ev.preventDefault();
        const value = ev.target.value;
        let parent = [...this.state.parent];

        if (type === "p") {
            if (value === "") {
                this.getInitialLoad(); return;
            }
            this.setState((x) => ({
                children: [],
                parent: [],
                initialLoad: []
            }));
            parent = "0";
        } else {
            if (value === "") {
                this.getChild(parent[parent.length - 1], "c");
                return;
            }
            parent = parent[parent.length - 1].id;
        }
        fetch(this.utils.getServer() + "/admin" + "/searchCategory?parent=" + parent + "&query=" + value, {
            headers: {
                'Authorization': 'Bearer ' + this.utils.token(),
                "Content-Type": "application/json",
            }
        }).then(x => x.json()).then((response) => {
            if (response.statusCode === 200) {
                if (type === "p") {
                    this.setState({ initialLoad: response.value })
                } else {
                    this.setState({ children: response.value })
                }
            }
        })
    }

    select(ev, sleectedType) {
        if (ev.target.className === "hoverGlow") {
            const elements = document.querySelectorAll('.hoverGlow');
            elements.forEach((element) => {
                element.style.backgroundColor = 'rgb(242, 242, 242)';
            });
            ev.target.style.backgroundColor = "rgb(184, 183, 183)";
            this.setState({value: sleectedType})
            this.props.onChange(sleectedType);
        }
    }

    render() {
        return (
            <div>
                <div id="frame" >
                    <div id="header" className="p-3 mb-2 bg-primary text-dark">Category</div> <br />
                    <div id="badge" className="p-3 mb-2 bg-light text-dark">
                        {this.state.parent ? (
                            <div id="badgeList">
                                <div>{this.state.parent.map((i, j) => (
                                    <React.Fragment key={j + "2"}>
                                        <button onClick={() => {
                                            if (i.parent === null) {
                                                this.setState((x) => ({ parent: this.state.parent.filter(x => x.id === i.id) }))
                                            } else {
                                                // backward loop
                                                let loopLimit = this.state.parent.length - 1;
                                                while (true) {
                                                    const curr = this.state.parent[loopLimit];
                                                    if (curr.id === i.id) {
                                                        break;
                                                    }
                                                    this.setState((x) => ({
                                                        parent: this.state.parent.filter(x => x.id !== curr.id)
                                                    }))
                                                    loopLimit--;
                                                }
                                            }
                                            this.getChild(i, i.parent === null ? "p" : "c");
                                        }} className="btn btn-primary badge">{i.productCategory}</button>
                                    </React.Fragment>
                                ))}</div> </div>
                        ) : null}
                    </div> <br />
                    <div className="Frames" id="frame1">
                        <center>
                            <input onInput={(ev) => { this.search(ev, "p") }} className="form-control search_Cat" placeholder="search" />
                        </center> <br /> <br />
                        {this.state.initialLoad.length > 0 ? this.state.initialLoad.map((i, j) => (
                            <React.Fragment key={j}>
                                <div onClick={(ev) => { this.select(ev, i) }} className="catBtn">
                                    <div className="hoverGlow">
                                        {i.productCategory}
                                        <div onClick={(ev) => {
                                            this.getChild(i, "p", ev)
                                        }} className="expand">{">"}</div>
                                    </div>
                                </div>
                            </React.Fragment>
                        )) : null}
                        {this.state.initialLoad.length > 0 ? (
                            <div>
                                <div className="loadMore">
                                    <center>
                                        <span onClick={(ev) => { this.scrollPagination(ev, null) }}>
                                            Load More
                                        </span>
                                    </center>
                                </div>
                            </div>
                        ) : null}

                    </div>
                    {this.state.children.length > 0 ? (
                        <div className="Frames" id="frame2">
                            <center>
                                <input onInput={(ev) => { this.search(ev, "c") }} className="form-control search_Cat" placeholder="search" />
                            </center> <br /> <br />
                            {this.state.children.map((i, j) => (
                                <React.Fragment key={j + "1"}>
                                    <div onClick={(ev) => { this.select(ev, i) }} className="catBtn" style={{ cursor: "pointer" }}>
                                        <div className="hoverGlow">
                                            {i.productCategory}
                                            <div onClick={(ev) => {
                                                this.getChild(i, "c", ev)
                                            }} className="expand">{">"}</div>
                                        </div>
                                    </div>
                                </React.Fragment>
                            ))}
                            {this.state.children.length > 0 ? (<div className="loadMore">
                                <center>
                                    <span onClick={(ev) => { this.scrollPagination(ev, 1) }}>
                                        Load More
                                    </span>
                                </center>
                            </div>) : null}
                        </div>
                    ) : null}
                </div>
            </div>
        )
    }
}

export class AdminCategories extends Component {

    constructor(props) {
        super(props);
    }


    render() {
        return (
            <div>
                <AdminNav /> <br />
                <div id="seletCategoryFrame">
                    <Category />
                </div>
            </div>
        )
    }
}