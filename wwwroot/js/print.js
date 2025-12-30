window.printReceipt = () => {
    window.print();
};

window.playErrorSound = () => {
    var audio = new Audio('/Sounds/error.mp3');

    console.log("Beep! Product not found.");
};