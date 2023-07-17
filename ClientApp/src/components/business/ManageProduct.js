import React, { Component } from "react";
import { AddProduct } from "./AddProduct";
import { SellerDashboard } from "./SellerDashboard";
import { Services } from "../../utils/services";
import "../../style/product_manager.css";


export class ProductManager extends Component {
    constructor(props) {
        super(props);
        this.utils = new Services();
        this.state = {
            product: null,
            editProcut: null,
        }
        this.componentDidMount = this.componentDidMount.bind(this);
    }

    componentDidMount() {
        fetch(this.utils.getServer() + "/market/get_product", {
            headers: {
                'Authorization': 'Bearer ' + this.utils.token(),
                "Content-Type": "application/json",
            },
            method: "get",
        }).then(rsp => rsp.json()).then((response) => {
            this.setState({ product: response.value });
        })
    }

    render() {
        const minimal = (Id) => {
            return Id.substring(0, 5) + "...";
        }
        const dateFormatter = (date) => {
            const dateTime = new Date(date);

            const dateOptions = {
                year: 'numeric',
                month: 'long',
                day: 'numeric'
            };

            const timeOptions = {
                hour: 'numeric',
                minute: 'numeric',
                second: 'numeric'
            };

            const readableDate = dateTime.toLocaleDateString(undefined, dateOptions);
            const readableTime = dateTime.toLocaleTimeString(undefined, timeOptions);
            return <div>{readableDate + " " + readableTime}</div>
        }
        return (
            <div>
                <SellerDashboard />
                {this.state.editProcut && (
                    <div id="re_populate_shadow" className="shadow-sm p-3 mb-5 bg-white rounded">
                        <div onClick={() => { this.setState({ editProcut: null }) }} id="cancelLayOut">x</div>
                        <br />
                        <AddProduct reverse_engineer={true} populateData={this.state.product.filter(x => x.id == this.state.editProcut)} />
                    </div>
                )}
                {this.state.product && (<div>
                    <br />
                    <table className="table table-striped">
                        <thead>
                            <tr>
                                <th scope="col">Product Name</th>
                                <th scope="col">Id</th>
                                <th scope="col">Date/Time</th>
                                <th scope="col">Action</th>
                            </tr>
                        </thead>
                        {this.state.product.map((i, j) => {
                            return (
                                <React.Fragment key={i.id}>
                                    <tbody>
                                        <tr>
                                            <td>{i.name}</td>
                                            <td onClick={(ev) => { navigator.clipboard.writeText(i.id) }} style={{ cursor: "pointer" }}>{minimal(i.id)}</td>
                                            <td>{dateFormatter(i.addedDate)}</td>
                                            <td><button onClick={() => {
                                                this.setState({ editProcut: i.id })
                                            }} className="btn btn-outline-warning">Edit</button></td>
                                        </tr>
                                    </tbody>
                                </React.Fragment>
                            )
                        })}
                    </table>
                </div>)}
            </div>
        )
    }
}