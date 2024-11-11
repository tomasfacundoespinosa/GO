#include <stdio.h>
#include <stdbool.h>
#include <ctype.h>

#define SIZE 9
#define EMPTY 0
#define BLACK 1
#define WHITE 2

int board[SIZE][SIZE];
int score_black = 0, score_white = 0;

void initialize_board() {
    for (int i = 0; i < SIZE; i++) {
        for (int j = 0; j < SIZE; j++) {
            board[i][j] = EMPTY;
        }
    }
}

void print_board() {
    printf("  ");
    for (int i = 0; i < SIZE; i++) printf("%d ", i);
    printf("\n");

    for (int i = 0; i < SIZE; i++) {
        printf("%d ", i);
        for (int j = 0; j < SIZE; j++) {
            if (board[i][j] == EMPTY) printf(". ");
            else if (board[i][j] == BLACK) printf("B ");
            else if (board[i][j] == WHITE) printf("W ");
        }
        printf("\n");
    }
}

bool is_within_bounds(int x, int y) {
    return x >= 0 && x < SIZE && y >= 0 && y < SIZE;
}

bool has_liberties(int x, int y, int color, bool visited[SIZE][SIZE]) {
    if (!is_within_bounds(x, y) || visited[x][y] || board[x][y] != color) return false;
    visited[x][y] = true;

    if ((is_within_bounds(x-1, y) && board[x-1][y] == EMPTY) ||
        (is_within_bounds(x+1, y) && board[x+1][y] == EMPTY) ||
        (is_within_bounds(x, y-1) && board[x][y-1] == EMPTY) ||
        (is_within_bounds(x, y+1) && board[x][y+1] == EMPTY)) {
        return true;
    }

    return has_liberties(x-1, y, color, visited) ||
           has_liberties(x+1, y, color, visited) ||
           has_liberties(x, y-1, color, visited) ||
           has_liberties(x, y+1, color, visited);
}

void capture_group(int x, int y, int color) {
    if (!is_within_bounds(x, y) || board[x][y] != color) return;
    board[x][y] = EMPTY;

    capture_group(x-1, y, color);
    capture_group(x+1, y, color);
    capture_group(x, y-1, color);
    capture_group(x, y+1, color);
}

void check_and_capture(int x, int y, int opponent_color) {
    bool visited[SIZE][SIZE] = {false};
    if (!has_liberties(x, y, opponent_color, visited)) {
        capture_group(x, y, opponent_color);
        if (opponent_color == BLACK) score_white++;
        else score_black++;
    }
}

void place_stone(int x, int y, int color) {
    if (!is_within_bounds(x, y) || board[x][y] != EMPTY) {
        printf("Posición inválida. Intenta otra vez.\n");
        return;
    }
    
    board[x][y] = color;
    int opponent_color = (color == BLACK) ? WHITE : BLACK;

    if (is_within_bounds(x-1, y) && board[x-1][y] == opponent_color) check_and_capture(x-1, y, opponent_color);
    if (is_within_bounds(x+1, y) && board[x+1][y] == opponent_color) check_and_capture(x+1, y, opponent_color);
    if (is_within_bounds(x, y-1) && board[x][y-1] == opponent_color) check_and_capture(x, y-1, opponent_color);
    if (is_within_bounds(x, y+1) && board[x][y+1] == opponent_color) check_and_capture(x, y+1, opponent_color);
}

void count_controlled_areas() {
    for (int i = 0; i < SIZE; i++) {
        for (int j = 0; j < SIZE; j++) {
            if (board[i][j] == BLACK) score_black++;
            else if (board[i][j] == WHITE) score_white++;
        }
    }
}

void play_game() {
    int x, y, turn = BLACK;
    char input[10];

    initialize_board();

    while (1) {
        print_board();
        printf("Puntaje -> Negro: %d | Blanco: %d\n", score_black, score_white);

        printf("Jugador %s, ingresa las coordenadas (x y) para colocar tu piedra o 'p' para pasar turno: ",
               (turn == BLACK) ? "Negro" : "Blanco");
        
        fgets(input, sizeof(input), stdin);

        if (input[0] == 'p' || input[0] == 'P') {
            if (turn == WHITE) break;
            turn = (turn == BLACK) ? WHITE : BLACK;
            continue;
        }

        if (sscanf(input, "%d %d", &x, &y) == 2) {
            place_stone(x, y, turn);
            turn = (turn == BLACK) ? WHITE : BLACK;
        } else {
            printf("Entrada inválida. Intenta nuevamente.\n");
        }
    }

    count_controlled_areas();
    printf("Juego terminado.\nPuntaje final -> Negro: %d | Blanco: %d\n", score_black, score_white);
    printf("Ganador: %s\n", (score_black > score_white) ? "Negro" : (score_black < score_white) ? "Blanco" : "Empate");
}

int main() {
    play_game();
    return 0;
}