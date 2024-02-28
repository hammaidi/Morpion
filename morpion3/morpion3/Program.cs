using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace morpion3
{
    class Program
    {
        static int winOrdinateur = 0;
        static int winDeuxieme = 0;
        static int draw = 0;
        static void Main(string[] args)
        {
            Menu();
        }

        static void Menu()
        {
            string j1 = "ordiDeuxieme", j2 = "ordinateur";
            string type;
            winOrdinateur = 0;
            winDeuxieme = 0;
            draw = 0;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("MORPION\n\n");
            Console.WriteLine("1.\tJoueur VS Joueur\n2.\tJoueur VS IA\n3.\tIA VS IA");
            type = Console.ReadLine();
            if (type != "3") // si c'est IA VS IA on ne rentre pas ici 
            {
                Console.Write("Saisir le nom des joueurs...\n\tJ1 : ");
                j1 = Console.ReadLine();
            }            
            if (type == "1") // si c'est J VS J on rentre ici pour le 2eme joueur
            {
                Console.Write("\tJ2 : ");
                j2 = Console.ReadLine();
            }
            jouer(j1, j2, type);
        }
        static void jouer(string j1, string j2, string type)
        {
            int nbGame = 1;
            bool visible = true;
            if (type == "3") // parametres partie ia vs ia 
            {
                Console.Write("Combien de parties doit jouer l'IA : ");
                nbGame = int.Parse(Console.ReadLine());
                Console.Write("Coups visibles (o/n): ");
                if (Console.ReadLine() == "n")
                    visible = false;
            }
            for (int a = 0; a < nbGame; a++)
            {
                int choix, coups = 0, joueur = 1;
                bool win = false, dispo;
                //initialisation du plateau
                int[,] plateau = new int[3, 3];
                //initialisation des valeurs déjà saisies  : vide au début
                List<int> hits = new List<int>();
                if (visible)
                    Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;
                if (type == "3")
                    Console.Write("Partie {0} : ", a+1);
                if (visible)
                {
                    Console.WriteLine("\nVoici la position des cases que vous devrez taper pour jouer : \n");
                    Console.WriteLine("1  2  3\n4  5  6\n7  8  9\n");
                }                
                //début de partie
                while (coups < 9 && !win)
                {
                    do
                    {
                        // position où l'on joue
                        choix = choix_coups(type, joueur, j1, j2, coups, plateau);
                        //on vérifie si la case n'a pas déjà été saisie
                        dispo = check_dispo(choix, hits, joueur, type);
                    } while ((choix < 1 || choix > 9) || !dispo); // on retape si le joueur entre une valeur hors de [1,9]
                    switch (choix)
                    {
                        case 1:
                            plateau[0, 0] = joueur;
                            break;
                        case 2:
                            plateau[0, 1] = joueur;
                            break;
                        case 3:
                            plateau[0, 2] = joueur;
                            break;
                        case 4:
                            plateau[1, 0] = joueur;
                            break;
                        case 5:
                            plateau[1, 1] = joueur;
                            break;
                        case 6:
                            plateau[1, 2] = joueur;
                            break;
                        case 7:
                            plateau[2, 0] = joueur;
                            break;
                        case 8:
                            plateau[2, 1] = joueur;
                            break;
                        case 9:
                            plateau[2, 2] = joueur;
                            break;
                    }
                    hits.Add(choix); // on ajoute la case choisie dans la liste des coups déjà joués
                    coups++; // on incrémente le compteur des coups
                             //on dit que l'ia a joué
                    if (((type == "2" && joueur == 1) || (type == "3")) && visible)
                        Console.WriteLine("L'ordinateur joue...");
                    if (visible)
                        affichage(plateau);
                    win = win_check(plateau, joueur, j1, j2, coups, type);
                    joueur = (coups % 2) + 1; // formule permettant de connaître le joueur en cours
                }
            }
            if (type == "3")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nVictoire(s) de {0} : {1}", j2, winOrdinateur);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Victoire(s) de {0} : {1}", j1, winDeuxieme);
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("Nombre de match(s) nul(s) : {0}", draw);
                Console.ForegroundColor = ConsoleColor.White;
            }
            
            //si victoire ou fin du jeu : proposer une nouvelle partie
            newGame();

        }

        static int choix_coups(string type, int joueur, string j1, string j2, int coups, int[,] plateau)
        {
            int choix = 0;
            Random pos_IA = new Random();
            if (type == "1") // JVJ
            {
                if (joueur == 1)
                    Console.Write(j1 + " (J" + joueur + "), à toi : ");
                else
                    Console.Write(j2 + " (J" + joueur + "), à toi : ");
                choix = int.Parse(Console.ReadLine());
            }
            else if (type == "2") // JVIA
            {
                if (joueur == 1)
                {
                    //ia joue sans affichage pour ne pas surcharger car on est dans une boucle ;
                    if (coups == 0)
                        choix = pos_IA.Next(1, 10); // coup aléatoire de l'ia 
                    else
                        choix = play_IA(plateau);
                }
                else
                {
                    Console.Write(j1 + " (J" + joueur + "), à toi : ");
                    choix = int.Parse(Console.ReadLine());
                }
            }
            else // IA VS IA 
            {
                choix = play_IA(plateau);
            }
            return choix;
        }
        static bool win_check(int[,] plateau, int joueur, string j1, string j2, int coups, string type)
        {
            bool win = false;
            if (
                    ((plateau[0, 0] + plateau[0, 1] + plateau[0, 2]) == joueur * 3 && plateau[0, 0] == plateau[0, 1]) || //ligne * colonne
                    ((plateau[1, 0] + plateau[1, 1] + plateau[1, 2]) == joueur * 3 && plateau[1, 0] == plateau[1, 1]) ||
                    ((plateau[2, 0] + plateau[2, 1] + plateau[2, 2]) == joueur * 3 && plateau[2, 0] == plateau[2, 1]) ||
                    ((plateau[0, 0] + plateau[1, 0] + plateau[2, 0]) == joueur * 3 && plateau[0, 0] == plateau[1, 0]) ||
                    ((plateau[0, 1] + plateau[1, 1] + plateau[2, 1]) == joueur * 3 && plateau[0, 1] == plateau[1, 1]) ||
                    ((plateau[0, 2] + plateau[1, 2] + plateau[2, 2]) == joueur * 3 && plateau[0, 2] == plateau[1, 2]) ||
                    ((plateau[0, 0] + plateau[1, 1] + plateau[2, 2]) == joueur * 3 && plateau[0, 0] == plateau[1, 1]) ||
                    ((plateau[2, 0] + plateau[1, 1] + plateau[0, 2]) == joueur * 3 && plateau[2, 0] == plateau[1, 1])
                    )
            {
                win = true;
                if (type == "1")
                {
                    if (joueur == 1)
                        Console.WriteLine("Victoire de " + j1 + ", félicitations !");
                    else
                        Console.WriteLine("Victoire de " + j2 + ", félicitations !");
                }
                else if (type == "2")
                {
                    if (joueur == 1)
                        Console.WriteLine("Victoire de " + j2 + ", félicitations !");
                    else
                        Console.WriteLine("Victoire de " + j1 + ", félicitations !");
                }
                else if (type == "3") // ajout
                {
                    if (joueur == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Victoire de " + j2 + ", félicitations !");
                        winOrdinateur++;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                        
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Victoire de " + j1 + ", félicitations !");
                        winDeuxieme++;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            else if (coups == 9) // si match nul 
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("Match nul !");
                draw++;
            }
            return win;
        }

        static bool check_dispo(int choix, List<int> hits, int joueur, string type)
        {
            bool dispo = true;
            foreach (int val in hits)
            {
                if (val == choix)
                {
                    dispo = false;
                    if ((type == "2" && joueur == 2) || (type == "1"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("La case est déjà occupée !");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            return dispo;
        }
        static void newGame()
        {
            string next;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Nouveau jeu ? o/n");
            next = Console.ReadLine();
            if (next == "o")
                Menu();
        }
        static void affichage(int[,] p_plateau) // type = tableau 2 dimensions
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (p_plateau[i, j] == 1)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else if (p_plateau[i, j] == 2)
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("{0}  ", p_plateau[i, j]);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }

        static int play_IA(int[,] plateau)
        {
            Random r = new Random();
            int res = r.Next(1, 10); // de base si aucune condition optimale n'est trouvée
            int tot_ligne = 0;
            int tot_col = 0;
            int max = 0;
            int coord = 0;
            string coordo = "";
            bool playL = true, playC = true;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (plateau[i, j] == 2)
                        playL = false;
                    if (plateau[j, i] == 2)
                        playC = false;
                    if (plateau[i, j] == 1)
                        tot_ligne += plateau[i, j];
                    if (plateau[j, i] == 1)
                        tot_col += plateau[j, i];
                }
                if (tot_ligne > max && playL)
                {
                    max = tot_ligne;
                    coord = i;
                    coordo = "ligne";
                }
                if (tot_col > max && playC)
                {
                    max = tot_col;
                    coord = i;
                    coordo = "colonne";
                }
                tot_col = 0;
                tot_ligne = 0;
                playL = true;
                playC = true;
            }
            if (coordo == "ligne")
            {
                int index;
                List<int> pos_ligne = new List<int>();
                switch (coord)
                {
                    case 0:
                        pos_ligne.Add(1);
                        pos_ligne.Add(2);
                        pos_ligne.Add(3);
                        index = r.Next(pos_ligne.Count);
                        res = pos_ligne[index];
                        break;
                    case 1:
                        pos_ligne.Add(4);
                        pos_ligne.Add(5);
                        pos_ligne.Add(6);
                        index = r.Next(pos_ligne.Count);
                        res = pos_ligne[index];
                        break;
                    case 2:
                        pos_ligne.Add(7);
                        pos_ligne.Add(8);
                        pos_ligne.Add(9);
                        index = r.Next(pos_ligne.Count);
                        res = pos_ligne[index];
                        break;
                }
            }
            else if (coordo == "colonne")
            {
                int index;
                List<int> pos_ligne = new List<int>();
                switch (coord)
                {
                    case 0:
                        pos_ligne.Add(1);
                        pos_ligne.Add(4);
                        pos_ligne.Add(7);
                        index = r.Next(pos_ligne.Count);
                        res = pos_ligne[index];
                        break;
                    case 1:
                        pos_ligne.Add(2);
                        pos_ligne.Add(5);
                        pos_ligne.Add(8);
                        index = r.Next(pos_ligne.Count);
                        res = pos_ligne[index];
                        break;
                    case 2:
                        pos_ligne.Add(3);
                        pos_ligne.Add(6);
                        pos_ligne.Add(9);
                        index = r.Next(pos_ligne.Count);
                        res = pos_ligne[index];
                        break;
                }
            }
            else if ((plateau[0, 0] + plateau[1, 1] + plateau[2, 2]) == 2) // GESTION DES 2 DIAG EN DERNIER
            {
                int index;
                List<int> pos_diag = new List<int>();
                pos_diag.Add(1);
                pos_diag.Add(5);
                pos_diag.Add(9);
                index = r.Next(pos_diag.Count);
                res = pos_diag[index];
            }
            else if ((plateau[2, 0] + plateau[1, 1] + plateau[0, 2]) == 2)
            {
                int index;
                List<int> pos_diag = new List<int>();
                pos_diag.Add(7);
                pos_diag.Add(5);
                pos_diag.Add(3);
                index = r.Next(pos_diag.Count);
                res = pos_diag[index];
            }


            return res;
        }
    }
}

